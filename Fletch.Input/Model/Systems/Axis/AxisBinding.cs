namespace Fletch.Input.Model.Systems.Axis
{
    struct AxisBinding
    {
        /// <summary>
        /// The binding's axis name.
        /// </summary>
        public string AxisName { get; init; }

        /// <summary>
        /// Only used for controller and to identify which player's controller to bind too.
        /// </summary>
        /// <remarks>If -1, applies to all controllers</remarks>
        public int PlayerID { get; init; }

        public KeyCode KeyboardAxisPositive { get; init; }
        public KeyCode KeyboardAxisNegative { get; init; }

        public ControllerAxisCode ControllerAxisPositive { get; init; }
        public ControllerAxisCode ControllerAxisNegative { get; init; }

        public ControllerButtonCode ControllerButtonPositive { get; init; }
        public ControllerButtonCode ControllerButtonNegative { get; init; }

        public MouseAxisCode MouseAxisPositive { get; init; }
        public MouseAxisCode MouseAxisNegative { get; init; }

        public MouseButtonCode MouseButtonPositive { get; init; }
        public MouseButtonCode MouseButtonNegative { get; init; }
    }
}
