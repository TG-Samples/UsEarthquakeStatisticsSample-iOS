using System.Drawing;
using Foundation;
using UIKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MapSuiteEarthquakeStatistics
{
    internal class DataTableSource : UITableViewSource
    {
        private Collection<SectionModel> sections;
        public Action<UITableView, NSIndexPath> RowClick;

        public DataTableSource()
            : this(null)
        { }

        public DataTableSource(IEnumerable<SectionModel> rows)
        {
            this.sections = new Collection<SectionModel>();
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    this.sections.Add(item);
                }
            }
        }

        public Collection<SectionModel> Sections
        {
            get { return sections; }
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            RowModel currentModel = sections[indexPath.Section].Rows[indexPath.Row];

            UITableViewCell cell = tableView.DequeueReusableCell("cell") ??
                                   new UITableViewCell(UITableViewCellStyle.Subtitle, "Cell");

            switch (currentModel.CellAccessory)
            {
                case UITableViewCellAccessory.Checkmark:
                    cell.Accessory = currentModel.IsChecked ? currentModel.CellAccessory : UITableViewCellAccessory.None;
                    break;
                default:
                    cell.Accessory = currentModel.CellAccessory;
                    break;
            }

            cell.Tag = indexPath.Row;
            cell.TextLabel.Text = currentModel.Name;
            cell.AccessoryView = currentModel.AccessoryView;

            if (currentModel.CustomUI != null)
            {
                currentModel.CustomUI.Frame = currentModel.CustomUIBounds;
                cell.Add(currentModel.CustomUI);
            }

            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return sections.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return sections[(int)section].Rows.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return sections[(int)section].Title;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            if (RowClick != null) RowClick(tableView, indexPath);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            SectionModel currentSection = sections[(int)section];
            if (currentSection.HeaderHeight > 0) return currentSection.HeaderHeight;
            return UITableView.AutomaticDimension;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            RowModel currentModel = sections[indexPath.Section].Rows[indexPath.Row];
            if (currentModel.RowHeight > 0) return currentModel.RowHeight;
            return UITableView.AutomaticDimension;
        }
    }

    internal class SectionModel
    {
        private Collection<RowModel> rows;

        public SectionModel(string title)
            : this(title, null)
        { }

        public SectionModel(string title, IEnumerable<RowModel> rows)
        {
            this.Title = title;
            this.rows = new Collection<RowModel>();
            if (rows != null)
            {
                foreach (var item in rows)
                {
                    this.rows.Add(item);
                }
            }
        }

        public string Title { get; set; }

        public Collection<RowModel> Rows
        {
            get { return rows; }
        }

        public float HeaderHeight { get; set; }
    }

    internal class RowModel
    {
        private RectangleF customUIBounds;

        public RowModel(string name)
            : this(name, null)
        { }

        public RowModel(string name, UIImageView accessoryView)
        {
            this.Name = name;
            this.AccessoryView = accessoryView;
        }

        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public UIView CustomUI { get; set; }

        public RectangleF CustomUIBounds
        {
            get { return customUIBounds; }
            set { customUIBounds = value; }
        }

        public float RowHeight { get; set; }

        public UIView AccessoryView { get; set; }

        public UITableViewCellAccessory CellAccessory { get; set; }
    }

    internal class EarthquakeRow
    {
        public EarthquakeRow()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        private EarthquakeRow(string yearValue, string longitudeValue, string latitudeValue, string depthValue, string magnitudeValue, string locationValue)
        {
            YearValue = yearValue;
            LongitudeValue = longitudeValue;
            LatitudeValue = latitudeValue;
            DepthValue = depthValue;
            MagnitudeValue = magnitudeValue;
            LocationValue = locationValue;
        }

        public string YearValue { get; set; }

        public string LongitudeValue { get; set; }

        public string LatitudeValue { get; set; }

        public string DepthValue { get; set; }

        public string MagnitudeValue { get; set; }

        public string LocationValue { get; set; }

        public UIImageView AccessoryView { get; set; }

        public override string ToString()
        {
            return string.Format("Year: {0}. At: Lon: {1}, Lat: {2}. Depth: {3}. Magnitude: {4}.", YearValue,
                LongitudeValue ?? string.Empty, LatitudeValue ?? string.Empty, DepthValue ?? string.Empty, MagnitudeValue ?? string.Empty);
        }
    }
}