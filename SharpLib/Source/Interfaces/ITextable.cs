namespace SharpLib.Source.Interfaces
{
    /// <summary>
    /// Интерфейс сериализации объекта в строку
    /// </summary>
    public interface ITextable
    {
        /// <summary>
        /// Преобзование объекта в строку
        /// </summary>
        string ToText();
    }
}