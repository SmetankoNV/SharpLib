namespace SharpLib.Source.Interfaces
{
    /// <summary>
    /// Интерфейс сериализации объекта в байтовый буфер
    /// </summary>
    public interface IBufferable
    {
        /// <summary>
        /// Преобзование объекта в байтовый буфер
        /// </summary>
        byte[] ToBuffer();
    }
}