namespace org.zxteam.zxassist.screenshotter
{
	using org.zxteam.lib.reusable.wpf.unusual;
	using System;
	using System.Runtime.InteropServices;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Interop;
	using System.Windows.Media;

	public partial class ToolWindow : DynamicContextMenu
	{
		public ToolWindow()
		{
			InitializeComponent();
		}

		//private readonly Vector _cursorOffset = new Vector(-60, -60);
		//protected override Vector CursorOffset { get { return this._cursorOffset; } }

		//protected override bool HitTestBounds(Point hitTestPoint)
		//{
		//    Vector cursorOffset = this.CursorOffset;
		//    double x = cursorOffset.X - 10;
		//    double y = cursorOffset.Y - 10;
		//    Rect leftBottomRect = new Rect(this.Left - x, this.Top, this.Width + x, -y);
		//    Rect rightTopRect = new Rect(this.Left, this.Top - y, -x, this.Height + y);
		//    Rect rightBottomRect = new Rect(this.Left - x, this.Top - y, this.Width + x, this.Height + y);

		//    return leftBottomRect.Contains(hitTestPoint)
		//        || rightTopRect.Contains(hitTestPoint)
		//        || rightBottomRect.Contains(hitTestPoint);
		//}
	}
}
