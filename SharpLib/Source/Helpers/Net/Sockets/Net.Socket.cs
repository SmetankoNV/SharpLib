using System;
using System.Net.Sockets;
using SharpLib.Source.Extensions.String;
using SharpLib.Source.Helpers.Threads;

namespace SharpLib.Source.Helpers.Net.Sockets
{
    /// <summary>
    /// Базовый класс асинхронного сокета
    /// </summary>
    public class NetSocket : IDisposable
    {
        #region Константы

        /// <summary>
        /// Размер буфера (прием)
        /// </summary>
        public const int BUFFER_SIZE = 128 * 1024;

        /// <summary>
        /// Максимальное количество входящих соединенений
        /// </summary>
        public const int LISTEN_CONN_MAX = 1024;

        #endregion

        #region Поля

        /// <summary>
        /// Общий идентификатор (для отладки)
        /// </summary>
        private static readonly SharedId _sharedId;

        /// <summary>
        /// Асинхронная операция "Подключился клиент". Вынесена в переменную, т.к. это необходимо в режиме сервера
        /// </summary>
        private SocketAsyncEventArgs _acceptAsyncArgs;

        #endregion

        #region Свойства

        /// <summary>
        /// .NET сокет
        /// </summary>
        public Socket Sock { get; private set; }

        /// <summary>
        /// Признак открытого/запущенного сокета
        /// </summary>
        public bool IsRunning => State == NetSocketState.Opened || State == NetSocketState.Listen;

        /// <summary>
        /// Состояние сокета
        /// </summary>
        public NetSocketState State { get; protected set; }

        /// <summary>
        /// Идентификатор сокета
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Текущий протокол сокета
        /// </summary>
        public NetProto Proto { get; }

        /// <summary>
        /// Локальный адрес
        /// </summary>
        public NetAddr LocalPoint { get; private set; }

        /// <summary>
        /// Удаленный адрес
        /// </summary>
        public NetAddr RemotePoint { get; private set; }

        #endregion

        #region События

        /// <summary>
        /// Соединение установлено (событие сокета в режиме сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnAccept;

        /// <summary>
        /// Сокет закрыт (по удаленной инициативе)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnBreak;

        /// <summary>
        /// Сокет закрыт (локальная инициатива)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnClose;

        /// <summary>
        /// Соединение установлено (событие сокета в режиме клиента)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnConnect;

        /// <summary>
        /// Соединение разорвано
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnDisconnect;

        /// <summary>
        /// Ошибка в работе сокета
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnError;

        /// <summary>
        /// Сокет открыт (событие сокета в режиме сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnListen;

        /// <summary>
        /// Получены данные
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnReceive;

        /// <summary>
        /// Данные отправлены
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnSend;

        #endregion

        #region Конструктор

        static NetSocket()
        {
            _sharedId = new SharedId();
        }

        public NetSocket(NetProto proto)
        {
            OnConnect = null;
            OnDisconnect = null;
            OnBreak = null;
            OnListen = null;
            OnClose = null;
            OnAccept = null;
            OnReceive = null;
            OnSend = null;
            OnError = null;

            State = NetSocketState.Closed;
            Id = _sharedId.GetNext();
            Sock = null;
            Proto = proto;
            LocalPoint = new NetAddr();
            RemotePoint = new NetAddr();
            _acceptAsyncArgs = null;
        }

        public NetSocket() : this(NetProto.Tcp4)
        {
        }

        public NetSocket(Socket sock) : this()
        {
            Sock = sock;
            Proto = ConvertToProtoTyp(sock.ProtocolType);
            State = NetSocketState.Opened;
            LocalPoint = sock.LocalEndPoint.ToNetAddrEx();
            RemotePoint = sock.RemoteEndPoint.ToNetAddrEx();
        }

        #endregion

        #region Методы

        public void Dispose()
        {
            Sock?.Dispose();
            _acceptAsyncArgs.Dispose();
        }

        /// <summary>
        /// Текстовое представление объекта
        /// </summary>
        public override string ToString()
        {
            string text = $"[{Id}] {LocalPoint} -> {RemotePoint}";

            return text;
        }

        /// <summary>
        /// Преобразование типа протокола во внутренний формат
        /// </summary>
        private NetProto ConvertToProtoTyp(ProtocolType typ)
        {
            switch (typ)
            {
                case ProtocolType.IP:
                    return NetProto.Ipv4;
                case ProtocolType.IPv4:
                    return NetProto.Ipv4;
                case ProtocolType.IPv6:
                    return NetProto.Ipv6;
                case ProtocolType.Udp:
                    return NetProto.Udp;
                case ProtocolType.Tcp:
                    return NetProto.Tcp4;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Преобразование типа протокола во внешний формат
        /// </summary>
        private ProtocolType ConvertFromoProtoTyp(NetProto typ)
        {
            switch (typ)
            {
                case NetProto.Ipv4:
                    return ProtocolType.IP;
                case NetProto.Ipv6:
                    return ProtocolType.IPv6;
                case NetProto.Udp:
                    return ProtocolType.Udp;
                case NetProto.Tcp4:
                    return ProtocolType.Tcp;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Генерация события "Соединение установлено"
        /// </summary>
        protected void RaiseEventConnect(Socket sock)
        {
            try
            {
                OnConnect?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Соединение разорвано (инициатива удаленной точки)"
        /// </summary>
        protected void RaiseEventDisconnect(Socket sock)
        {
            try
            {
                OnDisconnect?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Соединение разорвано (инициатива локальная)"
        /// </summary>
        protected void RaiseEventBreak(Socket sock)
        {
            try
            {
                OnBreak?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Ошибка в процессе работы"
        /// </summary>
        protected void RaiseEventError(Socket sock, SocketError error)
        {
            try
            {
                OnError?.Invoke(this, new NetSocketEventArgs(sock, error, null));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Получены данные"
        /// </summary>
        protected void RaiseEventReceive(Socket sock, byte[] buf)
        {
            try
            {
                OnReceive?.Invoke(this, new NetSocketEventArgs(sock, SocketError.Success, buf));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Переданы данные"
        /// </summary>
        protected void RaiseEventSend(Socket sock)
        {
            try
            {
                OnSend?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Сокет открыт для приема входящих соединений"
        /// </summary>
        protected void RaiseEventListen(Socket sock)
        {
            try
            {
                OnListen?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Сокет закрыт"
        /// </summary>
        protected void RaiseEventClose(Socket sock)
        {
            try
            {
                OnClose?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Генерация события "Установлено входящее соединение"
        /// </summary>
        protected void RaiseEventAccept(Socket sock)
        {
            try
            {
                OnAccept?.Invoke(this, new NetSocketEventArgs(sock));
            }
            catch
            {
                // Ошибка обработки в пользовательском коде
            }
        }

        /// <summary>
        /// Проверка завершена ли операция асинхронная операция синхронно
        /// </summary>
        private void CheckSyncCompleted(bool result, object sender, SocketAsyncEventArgs args)
        {
            if (result == false)
            {
                // Асинхронный вызов завершился синхронно: Вызов обработчика напрямую
                OnSocketAsyncEventCompleted(sender, args);
            }
        }

        /// <summary>
        /// Обработка асинхронных событий
        /// </summary>
        private void OnSocketAsyncEventCompleted(object sender, SocketAsyncEventArgs args)
        {
            var sock = args.UserToken as Socket;
            var error = args.SocketError;
            var oper = args.LastOperation;

            switch (oper)
            {
                case SocketAsyncOperation.None:
                    break;

                // Соединение установлено
                case SocketAsyncOperation.Connect:
                    {
                        if (error == SocketError.Success && sock != null)
                        {
                            DoConnectedCompete(sock, sender, args);
                        }
                        else
                        {
                            // Ошибка установки соединения: Генерация события
                            RaiseEventError(sock, error);
                            // Смена состояния
                            State = NetSocketState.Closed;
                        }
                    }
                    break;

                // Соединение закрыто
                case SocketAsyncOperation.Disconnect:
                    {
                        State = NetSocketState.Closed;

                        // Генерация события
                        RaiseEventDisconnect(sock);
                    }
                    break;

                // Данные приняты
                case SocketAsyncOperation.Receive:
                    {
                        if (sock != null)
                        {
                            var size = args.BytesTransferred;

                            // Принят буфер
                            if (size > 0)
                            {
                                var temp = args.Buffer;
                                var offset = args.Offset;
                                var buf = Mem.Clone(temp, offset, size);

                                // Передача данных приложению
                                RaiseEventReceive(sock, buf);

                                // Перевод в режим приема данных
                                var result = sock.ReceiveAsync(args);

                                // Проверка синхронного выполнения
                                CheckSyncCompleted(result, sender, args);

                                // Обязательный выход, чтобы избежать освобождения ресурсов в конце фукнции
                                return;
                            }

                            if (State == NetSocketState.Opened)
                            {
                                // Соединение разорвано по удаленной инициативе
                                State = NetSocketState.Closed;

                                // Оповещение приложения
                                RaiseEventBreak(sock);

                                // Удаление сокета    
                                sock.Close();
                            }
                            else if (State == NetSocketState.Closing)
                            {
                                // Соединение разорвано по локальной инициативе
                                State = NetSocketState.Closed;
                                // Оповещение приложения
                                RaiseEventDisconnect(sock);
                            }
                        }
                    }
                    break;

                // Данные переданы
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    {
                        if (error == SocketError.Success)
                        {
                            // Данные переданы успешно
                            RaiseEventSend(sock);
                        }
                        else
                        {
                            // Ошибка передачи
                            RaiseEventError(sock, error);
                        }
                    }
                    break;

                // Установлено входящее соединение
                case SocketAsyncOperation.Accept:
                    {
                        if (error == SocketError.Success)
                        {
                            // Получение сокета установленного соединения
                            sock = args.AcceptSocket;

                            // Ожидание следующего подключения
                            AcceptAsync(args);

                            // Генерация события
                            RaiseEventAccept(sock);

                            // Выход без Dispose асинхронного объекта
                            return;
                        }

                        // Соединение прервано (по инициативе клиента)
                        State = NetSocketState.Closed;

                        // Закрыто соединение
                        RaiseEventClose(sock);
                    }
                    break;
            } // end switch (анализ результата асинхронной операции)

            // ===============================================
            // 20130513 - Исправление ошибки утечки памяти
            // ===============================================
            args.Dispose();
        }

        /// <summary>
        /// Обработка установки соединения
        /// </summary>
        private void DoConnectedCompete(Socket sock, object sender, SocketAsyncEventArgs args)
        {
            // Установка буфера приема
            var buffer = new byte[BUFFER_SIZE];
            // Генерация нового асинхронного ожидания
            var async = new SocketAsyncEventArgs();
            async.UserToken = sock;
            async.SetBuffer(buffer, 0, buffer.Length);
            async.Completed += OnSocketAsyncEventCompleted;

            // Установка состояния
            State = NetSocketState.Opened;
            LocalPoint = sock.LocalEndPoint.ToNetAddrEx();
            RemotePoint = sock.RemoteEndPoint.ToNetAddrEx();

            // Генерация события
            RaiseEventConnect(sock);

            // Перевод в режим приема данных
            var result = sock.ReceiveAsync(async);

            // Проверка синхронного выполнения
            CheckSyncCompleted(result, sender, args);
        }

        /// <summary>
        /// Перевод сокета в асинхронное ожидание входящего соединения
        /// </summary>
        private void AcceptAsync(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            var isFinished = Sock.AcceptAsync(args);
            if (isFinished == false)
            {
                OnSocketAsyncEventCompleted(Sock, args);
            }
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(NetAddr addr)
        {
            Connect(addr.Ip.Text, addr.Port, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(string destIp, int destPort)
        {
            Connect(destIp, destPort, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(int destPort)
        {
            Connect(NetIpAddr.IF_LOOPBACK, destPort, 0);
        }

        /// <summary>
        /// Установка соединения
        /// </summary>
        public void Connect(string destIp, int destPort, int localPort, bool isSync = false)
        {
            if (IsRunning)
            {
                return;
            }

            var removeAddr = new NetAddr(destIp, destPort);

            if (Proto == NetProto.Udp)
            {
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                RemotePoint = removeAddr;
                State = NetSocketState.Opened;

                return;
            }

            State = NetSocketState.Opening;
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ConvertFromoProtoTyp(Proto));

            if (isSync)
            {
                // Синхронная установка соединения
                Sock.Connect(removeAddr.ToEndPointEx());
                // Обраотка установленного соединения
                DoConnectedCompete(Sock, null, null);
            }
            else
            {
                var async = new SocketAsyncEventArgs();
                async.UserToken = Sock;
                async.RemoteEndPoint = removeAddr.ToEndPointEx();
                async.Completed += OnSocketAsyncEventCompleted;

                // Асинхронное соединение с сервером и обработка отвека в OnSocketAsyncEventCompleted
                var result = Sock.ConnectAsync(async);

                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        /// <summary>
        /// Установка соединения (синхронно)
        /// </summary>
        public void ConnectSync(int destPort)
        {
            Connect(NetIpAddr.IF_LOOPBACK, destPort, 0, true);
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void DisconnectInternal(bool isSync)
        {
            if (IsRunning)
            {
                State = NetSocketState.Closing;

                if (isSync)
                {
                    Sock.Close(100);
                }
                else
                {
                    var async = new SocketAsyncEventArgs();
                    async.Completed += OnSocketAsyncEventCompleted;
                    var result = Sock.DisconnectAsync(async);
                    CheckSyncCompleted(result, Sock, async);
                }
            }
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void Disconnect()
        {
            DisconnectInternal(false);
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void DisconnectSync()
        {
            DisconnectInternal(true);
        }

        /// <summary>
        /// Перевод сокета в режим приема входящих соединений 
        /// </summary>
        public void Listen(int port)
        {
            if (IsRunning)
            {
                return;
            }

            LocalPoint = new NetAddr(port);

            if (Proto == NetProto.Udp)
            {
                Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Sock.Bind(LocalPoint.ToEndPointEx());

                State = NetSocketState.Listen;

                Receive();
            }

            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Sock.Bind(LocalPoint.ToEndPointEx());
            Sock.Listen(LISTEN_CONN_MAX);

            State = NetSocketState.Listen;

            _acceptAsyncArgs = new SocketAsyncEventArgs();
            _acceptAsyncArgs.Completed += OnSocketAsyncEventCompleted;
            _acceptAsyncArgs.UserToken = Sock;

            // Перевод сокета в режим ожидания подключений
            AcceptAsync(_acceptAsyncArgs);

            // Генерация события
            RaiseEventListen(Sock);
        }

        /// <summary>
        /// Закрытие сокета
        /// </summary>
        public void Close()
        {
            if (IsRunning)
            {
                State = NetSocketState.Closing;

                Sock.Close();
            }
        }

        public void SendText(string value)
        {
            var buffer = value.ToBytesEx();

            SendBuffer(buffer);
        }

        public void SendBuffer(byte[] value)
        {
            if (IsRunning)
            {
                bool result;

                var async = new SocketAsyncEventArgs();
                async.SetBuffer(value, 0, value.Length);
                async.UserToken = Sock;
                async.Completed += OnSocketAsyncEventCompleted;

                if (Proto == NetProto.Udp)
                {
                    async.RemoteEndPoint = RemotePoint.ToEndPointEx();
                    result = Sock.SendToAsync(async);
                }
                else
                {
                    result = Sock.SendAsync(async);
                }

                // Проверка синхронного выполнения
                CheckSyncCompleted(result, Sock, async);
            }
        }

        public void Receive()
        {
            // Установка буфера приема
            var buffer = new byte[BUFFER_SIZE];
            // Генерация нового асинхронного ожидания
            var async = new SocketAsyncEventArgs();
            async.UserToken = Sock;
            async.SetBuffer(buffer, 0, buffer.Length);
            async.Completed += OnSocketAsyncEventCompleted;

            // Перевод в режим приема данных
            var result = Sock.ReceiveAsync(async);

            // Проверка результата выполнения
            CheckSyncCompleted(result, this, async);
        }

        #endregion
    }
}