using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MapInfo.Mapping;
using MapInfo.Geometry;
using MapInfo.Tools;
using MapInfo.Styles;
using MapInfo.Data;
using MapInfo.Engine;

namespace MapAPP
{
    public partial class MainMapForm : Form
    {
        CoordSys cs = null;
        FeatureLayer pointLayer = null;
        BitmapPointStyle NodeStyle = null;
        public MainMapForm()
        {
            InitializeComponent();
            mapControl1.Map.ViewChangedEvent += new MapInfo.Mapping.ViewChangedEventHandler(Map_ViewChangedEvent);
            Map_ViewChangedEvent(this, null);
            mapControl1.Tools.FeatureSelected += new FeatureSelectedEventHandler(Tools_FeatureSelected);
        }

        void Tools_FeatureSelected(object sender, FeatureSelectedEventArgs e)
        {
            //MessageBox.Show("选中了了");
            if (e.Selection.Count>0)
            {
                IResultSetFeatureCollection fSelectCollection=e.Selection[0] ;
                if (fSelectCollection[0].Table.Alias == "TablePoint_selection")
                {
                    int id = Convert.ToInt32(fSelectCollection[0]["id"]);
                    InfoWindow frmInfoWindow = new InfoWindow(id);
                    frmInfoWindow.ShowDialog();
                    Session.Current.Selections.DefaultSelection.Clear();
                }
            }
        }

        void Map_ViewChangedEvent(object sender, MapInfo.Mapping.ViewChangedEventArgs e)
        {
            // Display the zoom level
            Double dblZoom = System.Convert.ToDouble(String.Format("{0:E2}", mapControl1.Map.Zoom.Value));
            if (statusStrip1.Items.Count > 0)
            {
                statusStrip1.Items[0].Text = "缩放: " + dblZoom.ToString() + " " + MapInfo.Geometry.CoordSys.DistanceUnitAbbreviation(mapControl1.Map.Zoom.Unit);
            }
        }

        private void MapForm1_Load(object sender, EventArgs e)
        {
            //加载地图
            string MapPath = Path.Combine(Application.StartupPath, @"MapData\World.mws");
            MapWorkSpaceLoader mwsLoader = new MapWorkSpaceLoader(MapPath);
            mapControl1.Map.Load(mwsLoader);

            foreach (IMapLayer layer in mapControl1.Map.Layers)
            {
                LayerHelper.SetSelectable(layer, false);

                //if (layer is FeatureLayer)
                //{
                //    LayerHelper.SetSelectable(layer, true);

                //}
            }

            cs = mapControl1.Map.GetDisplayCoordSys();//获取地图坐标系

            LoadPoint();//加载数据库数据
        }

        void LoadPoint()
        {
            DataTable dtTemp = null;
            try
            {
                dtTemp = DBHelper.Instance.GetDataTable("SELECT id,[x],[y],[t] FROM t_point");
            }
            catch
            {
                MessageBox.Show("加载数据失败！", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            
            MapInfo.Data.TableInfoMemTable tbEuqip = new MapInfo.Data.TableInfoMemTable("TablePoint");
            tbEuqip.Columns.Add(MapInfo.Data.ColumnFactory.CreateFeatureGeometryColumn(cs));//向表信息中添加必备的可绘图列
            tbEuqip.Columns.Add(MapInfo.Data.ColumnFactory.CreateStyleColumn());

            tbEuqip.Columns.Add(MapInfo.Data.ColumnFactory.CreateStringColumn("id", 40)); //设备编码
            tbEuqip.Columns.Add(MapInfo.Data.ColumnFactory.CreateStringColumn("name", 50)); //设备名称


            MapInfo.Data.Table table = MapInfo.Engine.Session.Current.Catalog.GetTable("TablePoint");//确保当前目录下不存在同名表
            if (table != null)
            {
                MapInfo.Engine.Session.Current.Catalog.CloseTable("TablePoint");
            }
            table = MapInfo.Engine.Session.Current.Catalog.CreateTable(tbEuqip);

            pointLayer = new FeatureLayer(table, "TablePoint", "TablePoint");//创建图层(并关联表)
            LabelSource source = new LabelSource(table);//绑定Table
            source.DefaultLabelProperties.Caption = "name";//指定哪个字段作为显示标注
            source.DefaultLabelProperties.Style.Font.ForeColor = Color.Red;
            source.DefaultLabelProperties.CalloutLine.Use = false;  //是否使用标注线  
            source.DefaultLabelProperties.Layout.Offset = 5;//标注偏移   
            LabelLayer labelLayer = new LabelLayer();
            labelLayer.Sources.Append(source);//加载指定数据
            mapControl1.Map.Layers.Add(pointLayer);
            mapControl1.Map.Layers.Add(labelLayer);

            //LayerHelper.SetEditable(equipLayer, true);
            LayerHelper.SetSelectable(pointLayer, true);

            if (dtTemp == null || dtTemp.Rows.Count <= 0)
                return;

            //MapTool.SetInfoTipExpression(mapControl1.Tools.MapToolProperties, pointLayer, "'设备状态：'+EquipState");

            NodeStyle = new MapInfo.Styles.BitmapPointStyle("TARG1-32.BMP", BitmapStyles.None, Color.Blue, 10);

            Random rd = new Random();
            foreach (DataRow dr in dtTemp.Rows)
            {
                Feature PointNode = new Feature(pointLayer.Table.TableInfo.Columns);
                //double x = Convert.ToDouble(dr["x"]);
                //double y = Convert.ToDouble(dr["y"]);
                double x = rd.Next(0, 90);
                double y = rd.Next(0, 180);
                PointNode.Geometry = new MapInfo.Geometry.Point(mapControl1.Map.GetDisplayCoordSys(), x,y);
                PointNode["id"] = dr["id"];
                PointNode["name"] = dr["t"];
                PointNode.Style = NodeStyle;
                table.InsertFeature(PointNode);
            }
        }

        private void mapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            DPoint pt = new DPoint();

            mapControl1.Map.DisplayTransform.FromDisplay(e.Location, out pt);

            toolStripStatusLabel2.Text = string.Format("x={0} y={1}", pt.x, pt.y);
        }
    }
}