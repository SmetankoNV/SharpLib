﻿namespace SharpLib.Source.Helpers.Files
{
    /// <summary>
    /// По логике: все файл, определяется тип: файл, директория, ссылка и т.д.
    /// </summary>
    public enum FileTyp
    {
        Unknown = 0,

        File,

        Folder,

        LinkFile,

        LinkFolder
    }
}
