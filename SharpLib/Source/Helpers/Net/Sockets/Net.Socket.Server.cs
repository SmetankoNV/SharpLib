using System;
using System.Collections.Generic;

namespace SharpLib.Source.Helpers.Net.Sockets
{
    /// <summary>
    /// Серверный сокет
    /// </summary>
    public class NetSocketServer : IDisposable
    {
        #region Поля

        private readonly NetSocket _listenSocket;

        #endregion

        #region Свойства

        /// <summary>
        /// Список сокетов, установивших соединение
        /// </summary>
        public List<NetSocket> Sockets { get; }

        /// <summary>
        /// Номер порта
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Признак открытого/запущенного сокета
        /// </summary>
        public bool IsRunning => State == NetSocketState.Opened || State == NetSocketState.Listen;

        /// <summary>
        /// Состояние сокета
        /// </summary>
        public NetSocketState State => _listenSocket.State;

        #endregion

        #region События

        /// <summary>
        /// Установлено соединение
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnConnect;

        /// <summary>
        /// Разорвано соединение (инициатива клиента)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnDisconnect;

        /// <summary>
        /// Разорвано соединение (инициатива сервера)
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnBreak;

        /// <summary>
        /// Сокет переведен в режим приема входящих соединений
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnListen;

        /// <summary>
        /// Сокет закрыт
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnClose;

        /// <summary>
        /// Приняты данные от удаленной точки
        /// </summary>
        public event EventHandler<NetSocketEventArgs> OnReceive;

        #endregion

        #region Конструктор

        public NetSocketServer()
        {
            OnListen = null;
            OnClose = null;
            OnConnect = null;
            OnReceive = null;

            _listenSocket = new NetSocket();
            _listenSocket.OnAccept += SocketOnAccept;
            _listenSocket.OnListen += SocketOnListen;
            _listenSocket.OnClose += SocketOnClose;

            Sockets = new List<NetSocket>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Сокет переведен в режим приема соединений"
        /// </summary>
        private void SocketOnListen(object sender, NetSocketEventArgs args)
        {
            OnListen?.Invoke(sender, args);
        }

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Сокет закрыт"
        /// </summary>
        private void SocketOnClose(object sender, NetSocketEventArgs args)
        {
            OnClose?.Invoke(sender, args);
        }

        /// <summary>
        /// Обработка СЕРВЕРНОГО сокета "Установлено входящее соединение"
        /// </summary>
        private void SocketOnAccept(object sender, NetSocketEventArgs args)
        {
            // Создание нового объекта SharpLib-сокета, использующего указанный .NET-сокет
            var newSocket = new NetSocket(args.Sock);
            newSocket.OnBreak += SocketOnDisconnect;
            newSocket.OnReceive += SocketOnReceive;
            newSocket.OnDisconnect += SocketOnBreak;

            // Добавление в список нового сокета
            Sockets.Add(newSocket);
            // Перевод сокета в режим приема данных
            newSocket.Receive();

            // Передача события приложению
            OnConnect?.Invoke(newSocket, args);
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Получены данные"
        /// </summary>
        private void SocketOnReceive(object sender, NetSocketEventArgs args)
        {
            OnReceive?.Invoke(sender, args);
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Соединение разорвано (по инициативе удаленной точки)"
        /// </summary>
        private void SocketOnDisconnect(object sender, NetSocketEventArgs args)
        {
            OnDisconnect?.Invoke(sender, args);

            // Удаление сокета из списка
            var socket = sender as NetSocket;
            Sockets.Remove(socket);
        }

        /// <summary>
        /// Обработка КЛИЕНТСКОГО сокета "Соединение разорвано (по инициативе сервера)"
        /// </summary>
        private void SocketOnBreak(object sender, NetSocketEventArgs args)
        {
            OnBreak?.Invoke(sender, args);
        }

        /// <summary>
        /// Перевод сервера в режим приема входящих соединений
        /// </summary>
        public void Listen(int port)
        {
            _listenSocket.Listen(port);
            Port = port;
        }

        /// <summary>
        /// Закрытие сокета
        /// </summary>
        public void Close()
        {
            // Закрытие текущих клиентских соединений 
            foreach (var socket in Sockets)
            {
                socket.Close();
            }

            Sockets.Clear();

            // Закрытие текущего соединения
            _listenSocket.Close();
        }

        /// <summary>
        /// Освобождение managed-ресурсов
        /// </summary>
        public void Dispose()
        {
            foreach (var socket in Sockets)
            {
                socket.Dispose();
            }

            _listenSocket.Dispose();
        }

        #endregion
    }
}