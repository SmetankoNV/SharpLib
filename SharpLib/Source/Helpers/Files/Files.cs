using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Files
{
    /// <summary>
    /// Класс содержит вспомогательные методы работы с файлами, директориями
    /// </summary>
    public static class Files
    {
        #region Поля

        public static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

        #endregion

        #region Методы

        /// <summary>
        /// Добавление разделителя пути (если необходимо)
        /// <example>
        /// Input:  'C:\Folder 1'
        /// Result: 'C:\Folder 1\'
        /// </example>
        /// </summary>
        public static string AddPathSeparator(string path)
        {
            if (path.IsValid())
            {
                if (path.EndsWith(PathSeparator) == false)
                {
                    path += PathSeparator;
                }
            }

            return path;
        }

        /// <summary>
        /// Удаление последнего разделителя (если он есть)
        /// </summary>
        /// <example>
        /// Input:  'C:\Folder 1\'
        /// Result: 'C:\Folder 1'
        /// </example>
        public static string RemoveLastSeparator(string path)
        {
            return path.TrimEnd('/', '\\');
        }

        /// <summary>
        /// Формирование относительного пути
        /// </summary>
        public static string GetPathRelative(string path1, string path2, bool removeLastDelimetr = false)
        {
            path1 = AddPathSeparator(path1);
            path2 = AddPathSeparator(path2);

            // Определение минимальной вложенности
            var uri1 = new Uri(path1);
            var uri2 = new Uri(path2);
            var uriRel = uri1.MakeRelativeUri(uri2);
            var result = uriRel.ToString();

            if (removeLastDelimetr)
            {
                result = RemoveLastSeparator(result);
            }

            result = Uri.UnescapeDataString(result);

            return result;
        }

        /// <summary>
        /// Определение абсолютного пути на основании основного
        /// <example>
        /// basePath     = "C:\Folder 1\Folder 2\"
        /// relativaPath = "..\file.txt"
        /// result       = "C:\Folder 1\file.txt
        /// </example>
        /// </summary>
        public static string GetPathAbsolute(string basePath, string relativePath)
        {
            if (IsFile(basePath))
            {
                basePath = GetDirectory(basePath);
            }

            basePath = AddPathSeparator(basePath);
            if (relativePath.IsNotValid())
            {
                return basePath;
            }

            var removeLastDelimetr = false;
            if (Directory.Exists(relativePath))
            {
                relativePath = AddPathSeparator(relativePath);
            }
            else
            {
                removeLastDelimetr = true;
            }

            var baseUri = new Uri(basePath);
            var absUri = new Uri(baseUri, relativePath);
            var result = absUri.LocalPath;

            if (removeLastDelimetr)
            {
                result = RemoveLastSeparator(result);
            }

            return result;
        }

        /// <summary>
        /// Извлечение имени файла БЕЗ расширения из полного имени и пути
        /// </summary>
        public static string GetFileName(string filepath)
        {
            string text;

            if (IsDirectory(filepath))
            {
                var dinfo = new DirectoryInfo(filepath);

                text = dinfo.Name;
            }
            else
            {
                text = Path.GetFileNameWithoutExtension(filepath);
            }

            return text;
        }

        /// <summary>
        /// Чтение расширения файла (без '.'. Например: avi)
        /// </summary>
        public static string GetExtension(string filepath)
        {
            // ReSharper disable PossibleNullReferenceException
            return Path.GetExtension(filepath).TrimStart('.');
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Чтение директории файла (для директории возвращает ее саму)
        /// </summary>
        /// <remarks>
        /// C:/1.txt => C:/
        /// </remarks>
        public static string GetDirectory(string location)
        {
            if (Directory.Exists(location))
            {
                return location;
            }

            if (File.Exists(location))
            {
                return Path.GetDirectoryName(location);
            }

            // Элемент не существует - Считается что файл
            return Path.GetDirectoryName(location);
        }

        /// <summary>
        /// Чтение родительской директории
        /// </summary>
        public static string GetDirectoryParent(string location)
        {
            try
            {
                location = GetDirectory(location);

                var parent = Directory.GetParent(location);

                if (parent != null)
                {
                    return parent.FullName;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Создание файла
        /// </summary>
        public static bool CreateFile(string location)
        {
            try
            {
                var stream = File.Create(location);
                stream.Dispose();

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Создание файла
        /// </summary>
        public static string CreateFile(string parent, string name)
        {
            var path = Path.Combine(parent, name);

            CreateFile(path);

            return path;
        }

        /// <summary>
        /// Создание каталога
        /// </summary>
        public static bool CreateDirectory(string location)
        {
            try
            {
                Directory.CreateDirectory(location);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Создание каталога в указанном родительском
        /// </summary>
        public static string CreateDirectory(string parent, string name)
        {
            var path = Path.Combine(parent, name);

            CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Проверка является ли путь директорией
        /// </summary>
        public static bool IsDirectory(string path)
        {
            return CheckTyp(path) == FileTyp.Folder;
        }

        /// <summary>
        /// Проверка является ли путь файлом
        /// </summary>
        public static bool IsFile(string path)
        {
            return CheckTyp(path) == FileTyp.File;
        }

        /// <summary>
        /// Извлечение имени файла из полного имени и пути
        /// </summary>
        public static string GetFileNameAndExt(string filepath)
        {
            var text = Path.GetFileName(filepath);

            return text;
        }

        /// <summary>
        /// Извлечение имени файла из полного имени и пути
        /// </summary>
        public static string GetEntryName(string location)
        {
            return GetLastEntry(location);
        }

        /// <summary>
        /// Чтение содержимого файла как текст
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <returns>Текстовое содержимого файла</returns>
        public static string ReadText(string filename)
        {
            try
            {
                using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var r = new StreamReader(stream))
                    {
                        return r.ReadToEnd();
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Чтение содержимого файла как буфер
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <returns>Массив байт</returns>
        public static byte[] ReadBuffer(string filename)
        {
            using (var b = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var size = (int)b.BaseStream.Length;
                b.BaseStream.Seek(0, SeekOrigin.Begin);

                var result = b.ReadBytes(size);

                return result;
            }
        }

        /// <summary>
        /// Чтение содержимого файла как буфер
        /// </summary>
        /// <param name="filename">Полный путь к файлу</param>
        /// <param name="offset">Смещение в файле</param>
        /// <param name="size">Размер читаемых данных</param>
        public static byte[] ReadBuffer(string filename, int offset, int size)
        {
            using (var b = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var length = (int)b.BaseStream.Length;
                var pos = offset > length ? length : offset;
                var remain = length - offset;
                size = System.Math.Min(remain, size);

                b.BaseStream.Seek(pos, SeekOrigin.Begin);

                var result = b.ReadBytes(size);

                return result;
            }
        }

        /// <summary>
        /// Запись содержимого файла как текст
        /// </summary>
        public static bool WriteText(string filename, string text)
        {
            try
            {
                File.WriteAllText(filename, text);

                return true;
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Запись содержимого файла как буфер
        /// </summary>
        public static void WriteBuffer(string filename, byte[] data)
        {
            File.WriteAllBytes(filename, data);
        }

        /// <summary>
        /// Сохранение потока в файл
        /// </summary>
        public static bool WriteStream(Stream stream, string filename)
        {
            return stream.WriteToFileEx(filename);
        }

        /// <summary>
        /// Копирование файла
        /// </summary>
        public static string CopyFile(string srcPath, string destPath, string newName = null)
        {
            if (string.IsNullOrEmpty(destPath))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(srcPath))
            {
                return string.Empty;
            }

            destPath = destPath.TrimEnd('\\');

            try
            {
                var filename = newName ?? GetFileNameAndExt(srcPath);
                var newPath = PathEx.Combine(destPath, filename);

                if (File.Exists(newPath))
                {
                    DeleteFile(newPath);
                }

                File.Copy(srcPath, newPath);

                return newPath;
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Копирование директории (с рекурсивным содержимым)
        /// </summary>
        public static string CopyDirectory(string srcDir, string destDir, string newName = null)
        {
            destDir = newName == null
                ? Path.Combine(destDir, GetFileName(srcDir))
                : Path.Combine(destDir, newName);

            if (Directory.Exists(destDir) == false)
            {
                Directory.CreateDirectory(destDir);
            }

            // Создание директорий назначений
            var dirs = GetDirectories(srcDir).ToList();
            foreach (var dirPath in dirs)
            {
                var newDir = dirPath.Replace(srcDir, destDir);
                Directory.CreateDirectory(newDir);
            }

            // Копирование всех файлов
            var files = GetFiles(srcDir).ToList();
            foreach (var srcPath in files)
            {
                var destFile = srcPath.Replace(srcDir, destDir);

                if (File.Exists(destFile))
                {
                    File.Delete(destFile);
                }

                File.Copy(srcPath, destFile);
            }

            return destDir;
        }

        /// <summary>
        /// Копирование записей
        /// </summary>
        public static void Copy(string srcLocation, string destDir, string newName = null)
        {
            switch (CheckTyp(srcLocation))
            {
                case FileTyp.File:
                    CopyFile(srcLocation, destDir, newName);
                    break;
                case FileTyp.Folder:
                    CopyDirectory(srcLocation, destDir, newName);
                    break;
            }
        }

        /// <summary>
        /// Чтение списка файлов в директории
        /// </summary>
        public static IEnumerable<string> GetFiles(string path, bool recursive = true, bool includeHidden = true, string mask = "*.*")
        {
            var result = Directory.GetFiles(path, mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (includeHidden == false && result.Any())
            {
                var filtered = result.Where(x => new DirectoryInfo(x).Attributes.HasFlag(FileAttributes.Hidden) == false);

                return filtered;
            }

            return result;
        }

        /// <summary>
        /// Чтение списка директорий в директории
        /// </summary>
        public static IEnumerable<string> GetDirectories(string path, bool recursive = true, bool includeHidden = true, string mask = "*")
        {
            var result = Directory.GetDirectories(path, mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (includeHidden == false && result.Any())
            {
                var filtered = result.Where(x => new DirectoryInfo(x).Attributes.HasFlag(FileAttributes.Hidden) == false);

                return filtered;
            }

            return result;
        }

        /// <summary>
        /// Чтение списка всех элементв в директории
        /// </summary>
        public static IEnumerable<string> GetEntries(string path, bool recursive = true, bool includeHidden = true, string mask = "*")
        {
            var files = GetFiles(path, recursive, includeHidden, mask);
            var dirs = GetDirectories(path, recursive, includeHidden, mask);

            return files.Concat(dirs);
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        public static bool DeleteFile(string filename, bool throwEx = false)
        {
            try
            {
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);

                return true;
            }
            catch
            {
                if (throwEx)
                {
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        /// Смена имени файла/директории
        /// </summary>
        public static string Rename(string path, string newName, bool onlyName = true)
        {
            if (path.IsValid() == false)
            {
                return string.Empty;
            }
            if (newName.IsValid() == false)
            {
                return string.Empty;
            }

            var typ = CheckTyp(path);

            if (typ == FileTyp.File)
            {
                // Путь является файлом
                // Составление полного пути получателя
                var ext = GetExtension(path);
                if (ext.IsValid())
                {
                    // Если расширение существует добавление разделителя
                    ext = "." + ext;
                }
                var destPath = onlyName
                    ? PathEx.Combine(GetDirectory(path), newName + ext)
                    : PathEx.Combine(GetDirectory(path), newName);

                // Файл уже так называется
                if (destPath == path)
                {
                    return destPath;
                }

                // Если файл существует: Удаление файла
                if (File.Exists(destPath))
                {
                    DeleteFile(destPath);
                }

                var info = new FileInfo(path);
                info.MoveTo(destPath);

                return destPath;
            }

            if (typ == FileTyp.Folder)
            {
                // Путь является директорией
                // Составление полного пути получателя
                var destPath = PathEx.Combine(GetDirectoryParent(path), newName);

                // Если файл существует: Удаление файла
                if (File.Exists(destPath))
                {
                    DeleteDirectory(destPath);
                }

                var info = new DirectoryInfo(path);
                info.MoveTo(destPath);

                return destPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Удаление директории/файла
        /// </summary>
        public static void Delete(string path)
        {
            if (path.IsValid() == false)
            {
                return;
            }

            switch (CheckTyp(path))
            {
                case FileTyp.File:
                    DeleteFile(path);
                    break;
                case FileTyp.Folder:
                    DeleteDirectory(path);
                    break;
            }
        }

        /// <summary>
        /// Удаление каталога
        /// </summary>
        public static void DeleteDirectory(string dirPath, bool onlyErase = false)
        {
            var listSubFolders = Directory.GetDirectories(dirPath);

            foreach (var subFolder in listSubFolders)
            {
                DeleteDirectory(subFolder);
            }

            var files = Directory.GetFiles(dirPath);
            foreach (var f in files)
            {
                var attr = File.GetAttributes(f);

                if (attr.HasFlag(FileAttributes.ReadOnly))
                {
                    File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                }

                File.Delete(f);
            }

            if (onlyErase == false)
            {
                Directory.Delete(dirPath);
            }
        }

        /// <summary>
        /// Очистка каталога
        /// </summary>
        public static void EraseDirectory(string dirPath)
        {
            DeleteDirectory(dirPath);

            // Ожидание завершения операции удаления каталога
            Thread.Sleep(50);

            CreateDirectory(dirPath);
        }

        /// <summary>
        /// Сброс атрибутов
        /// </summary>
        public static void ClearAttribute(string location, FileAttributes value)
        {
            var attributes = File.GetAttributes(location);

            if ((attributes & value) != 0)
            {
                // Есть атрибуты, которые нужно сбросить
                attributes &= ~value;

                File.SetAttributes(location, attributes);
            }
        }

        /// <summary>
        /// Создание временной директории в %TEMP%
        /// </summary>
        public static string GetTempDirectory(bool isCreate = true)
        {
            var path = Path.GetTempPath();

            if (isCreate)
            {
                path = Path.Combine(path, Path.GetRandomFileName());
                CreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// Генерация имени временного файла в %TEMP%
        /// </summary>
        public static string GetTempFilename(string startPart = null)
        {
            var path = Path.GetTempFileName();

            if (startPart.IsValid())
            {
                path = Path.Combine(
                    GetDirectory(path),
                    Path.GetFileName(startPart) + Path.GetFileName(path));
            }

            return path;
        }

        /// <summary>
        /// Чтение размера файла
        /// </summary>
        public static long GetFileSize(string location)
        {
            var info = new FileInfo(location);

            return info.Length;
        }

        /// <summary>
        /// Чтение размера директории
        /// </summary>
        public static long GetDirectorySize(string location)
        {
            // Recursively scan directories.
            var a = Directory.GetFiles(location, "*.*", SearchOption.AllDirectories);

            var size = a
                .Select(name => new FileInfo(name))
                .Select(info => info.Length)
                .Sum();

            return size;
        }

        /// <summary>
        /// Чтение размера Элемента
        /// </summary>
        public static long GetEntrySize(string location)
        {
            switch (CheckTyp(location))
            {
                case FileTyp.File:
                    return GetFileSize(location);
                case FileTyp.Folder:
                    return GetDirectorySize(location);
            }

            return 0;
        }

        /// <summary>
        /// Чтение времени создания записи
        /// </summary>
        public static DateTime GetCreateTime(string location)
        {
            switch (CheckTyp(location))
            {
                case FileTyp.File:
                    return new FileInfo(location).CreationTimeUtc;
                case FileTyp.Folder:
                    return Directory.GetCreationTimeUtc(location);
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Чтение времени последней модификации записи
        /// </summary>
        public static DateTime GetModifyTime(string location)
        {
            switch (CheckTyp(location))
            {
                case FileTyp.File:
                    return new FileInfo(location).LastWriteTimeUtc;
                case FileTyp.Folder:
                    return Directory.GetLastWriteTimeUtc(location);
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Определение типа записи файловой системы
        /// </summary>
        public static FileTyp CheckTyp(string location)
        {
            if (File.Exists(location))
            {
                var fileInfo = new FileInfo(location);

                if (fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    return FileTyp.LinkFile;
                }

                return FileTyp.File;
            }

            if (Directory.Exists(location))
            {
                return FileTyp.Folder;
            }

            return FileTyp.Unknown;
        }

        /// <summary>
        /// Проверка, что файл имеет аттрибут "Скрытый"
        /// </summary>
        public static bool IsHidden(string path)
        {
            var typ = CheckTyp(path);

            if (typ == FileTyp.File)
            {
                var attr = File.GetAttributes(path);

                return attr.HasFlag(FileAttributes.Hidden);
            }

            if (typ == FileTyp.Folder)
            {
                var info = new DirectoryInfo(path);

                return info.Attributes.HasFlag(FileAttributes.Hidden);
            }

            return false;
        }

        /// <summary>
        /// Чтение последней записи в полном пути
        /// </summary>
        /// <example>
        /// /path 1/path 2/file*.cpp => file*.cpp
        /// </example>
        public static string GetLastEntry(string path)
        {
            path = path.ToLinuxPathEx();
            var entries = path.Split('/');

            if (entries.Length > 0)
            {
                return entries[entries.Length - 1];
            }

            return string.Empty;
        }

        /// <summary>
        /// Получение списка элементов файловой системы по маске
        /// </summary>
        public static List<string> GetEntriesByMask(string root, string mask, bool recursive = false)
        {
            // Маска "Все вхождения"
            var all = mask == "*";
            // Маска "Все рекурсивные вхождения"
            var allr = mask == "**";

            List<string> entries;

            if (all)
            {
                entries = GetEntries(root, false).ToList();
            }
            else if (allr)
            {
                entries = GetEntries(root).ToList();
            }
            else
            {
                entries = GetEntries(root, recursive, false).ToList();
                entries = entries.Where(x => GetLastEntry(x).ContainsByMaskEx(mask)).ToList();
            }

            return entries;
        }

        /// <summary>
        /// Перенос файла из одного места в другое
        /// </summary>
        /// <param name="dir">Директория назначения</param>
        /// <param name="filename">Полный путь к исходному файлу</param>
        /// <returns></returns>
        public static string MoveFile(string dir, string filename)
        {
            var name = GetFileNameAndExt(filename);
            var destfile = PathEx.Combine(dir, name);
            File.Move(filename, destfile);

            return destfile;
        }

        /// <summary>
        /// Установка прав файлу
        /// </summary>
        /// <param name="file">Полный путь к файлу</param>
        /// <param name="permissions">Права в Linux (+w или 777)</param>
        public static bool Chmod(string file, string permissions)
        {
            if (Hardware.Hardware.Os.IsLinux)
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "/bin/chmod";
                process.StartInfo.Arguments = $"{permissions} {file}";
                process.StartInfo.UseShellExecute = false;

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }

            return true;
        }

        #endregion
    }
}