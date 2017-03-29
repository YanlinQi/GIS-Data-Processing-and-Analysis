using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostGISDemo
{
    static class Program
    {
        ///2015/10/22
        ///设计复杂点、线、多边形类
        ///完善其空间分析函数
        ///完成点、线的读入
        ///完成复杂点的点选

        ///2015/10/23
        ///修复点击缩小显示的Bug
        ///修复框选复杂点（多点）出错的问题 —— FExtent中SetValue的问题
        ///完成复杂线的点选、框选
        ///完成复杂多边形的读入
        ///完成复杂多边形点选、框选
        ///复杂多边形中多个内环的问题仍待解决！！！

        ///2010/10/30
        ///添加FShapefile类，实现直接打开Shapefile功能
        ///实现鼠标滚轮放大缩小操作

        ///2015/11/01
        /// 解决移动、选择时的闪烁问题（DoubleBuffer）
        /// 实现数据库界面
        /// 数据库导入、文件导入

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }
}
