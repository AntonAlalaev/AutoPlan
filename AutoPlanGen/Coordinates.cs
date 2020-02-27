namespace AutoPlan
{
    /// <summary>
    /// Геометрия секции
    /// </summary>
    internal class Coordinates
    {
        /// <summary>
        /// точка вставки
        /// </summary>
        public Point XY { get; set; }

        /// <summary>
        /// Верхний левый угол
        /// </summary>
        public Point TopLeft { get; set; }

        /// <summary>
        /// Верхний правый угол
        /// </summary>
        public Point TopRight { get; set; }

        /// <summary>
        /// Нижний левый угол
        /// </summary>
        public Point BottomLeft { get; set; }

        /// <summary>
        /// Нижний правый угол
        /// </summary>
        public Point BottomRight { get; set; }
    }
}