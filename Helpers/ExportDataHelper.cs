using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace WebForm.Helpers
{
    public class ExportDataHelpers
    {
        public DataTable DTTable { get; set; }          // Datable

        public static string FileName { get; set; }     //Ten file : danhsach.xls

        public string Title { get; set; }               //Tieu de noi dung file

        public string Save_File { get; set; }           //Tieu de noi dung file

        public int ExportRowTotal = 0;

        public Dictionary<string, string> Dictionary { get; set; }//List cac truong truyen vao

        /// <summary>
        /// Ham tra ve action result goi o controller
        /// </summary>
        /// <param name="p_Dataset">Dataset bao cao</param>
        /// <param name="pFileName">ten file : danhsach.xls</param>
        /// <param name="p_title">Title trong noi dung bao cao</param>
        /// <param name="p_srt_dic">string Dictionary nhung truong can hien thi </param>
        public ExportDataHelpers(DataTable p_DataTable, string pFileName, string p_title, Dictionary<string, string> p_Dictionary, string p_saveFile)
        {
            DTTable = p_DataTable;
            FileName = pFileName;
            Title = p_title;
            Dictionary = p_Dictionary;
            Save_File = p_saveFile;
            if (Dictionary == null)
            {
                Dictionary = new Dictionary<string, string>();
            }
        }

      
        /// <summary>
        /// Xu ly truong hop datatable nhieu hon 65k ban ghi
        /// </summary>
        /// <param name="p_ef">excel file truyen vao</param>
        /// <param name="p_dt">datatable truyen vao</param>
        /// <param name="p_start_row"> bat dau </param>
        /// <param name="p_to_row">ket thuc</param>
        /// <param name="NameReport">ten bao cao</param>        
        static void ExportDataSet65k(ref ExcelFile p_ef, DataTable p_dt, int p_start_row, int p_to_row, string NameReport, int p_sheet, Dictionary<string, string> p_dic_format)
        {
            try
            {
                int _sheet = p_sheet;
                DataTable _new_dt = new DataTable();
                _new_dt.TableName = NameReport + "_sheet" + (_sheet + 1).ToString();
                _new_dt = p_dt.Clone();
                for (int i = p_start_row; i < p_dt.Rows.Count; i++)
                {
                    if (i <= p_to_row)
                    {
                        DataRow _dr = (DataRow)p_dt.Rows[i];
                        _new_dt.Rows.Add(_dr.ItemArray);
                    }
                }

                // Add new worksheet to the file.
                var ws = p_ef.Worksheets.Add(_new_dt.TableName);
                ws.Cells["F2"].Value = NameReport.ToUpper();
                ws.Cells["F2"].Style.Font.Weight = ExcelFont.BoldWeight;

                // Insert the data from DataTable to the worksheet starting at cell "A5".
                ws.InsertDataTable(_new_dt, "A5", true);

                int stt = _new_dt.Columns.Count;//dem so cot trong dt

                p_ef.Worksheets[0].Columns[0].Width = 4 * 256;
                p_ef.Worksheets[0].Rows[4].Height = 2 * 256;
                for (int i = 0; i < stt; i++)
                {
                    //p_ef.Worksheets[0].Cells[4, i].Style.HorizontalAlignment = HorizontalAlignmentStyle.Fill;
                    p_ef.Worksheets[0].Cells[4, i].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
                    p_ef.Worksheets[0].Cells[4, i].Style.VerticalAlignment = VerticalAlignmentStyle.Center;
                    p_ef.Worksheets[0].Cells[4, i].Style.Borders.SetBorders(MultipleBorders.Top, System.Drawing.Color.FromArgb(0, 0, 0), LineStyle.Thin);
                    p_ef.Worksheets[0].Cells[4, i].Style.Borders.SetBorders(MultipleBorders.Bottom, System.Drawing.Color.FromArgb(0, 0, 0), LineStyle.Thin);
                    p_ef.Worksheets[0].Cells[4, i].Style.Borders.SetBorders(MultipleBorders.Left, System.Drawing.Color.FromArgb(0, 0, 0), LineStyle.Thin);
                    p_ef.Worksheets[0].Cells[4, i].Style.Borders.SetBorders(MultipleBorders.Right, System.Drawing.Color.FromArgb(0, 0, 0), LineStyle.Thin);
                    p_ef.Worksheets[0].Cells[4, i].Style.Font.Weight = ExcelFont.BoldWeight;
                    p_ef.Worksheets[0].Cells[4, i].Style.WrapText = true;

                    if (_new_dt.Columns[i].DataType == typeof(decimal) || _new_dt.Columns[i].DataType == typeof(int))
                    {
                        p_ef.Worksheets[_sheet].Columns[i].Style.NumberFormat = "#,##0";//#,#

                        int value = Convert.ToInt16(p_dic_format[_new_dt.Columns[i].ColumnName.ToUpper()]);
                        if (value == 1)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.0";//#,#
                        else if (value == 2)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.00";//#,#
                        else if (value == 3)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.000";//#,#
                        else if (value == 4)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.0000";//#,#
                        else if (value == 5)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.00000";//#,#
                        else if (value == 6)
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0.000000";
                        else
                            p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "#,##0";//#,#
                    }
                    if (_new_dt.Columns[i].DataType == typeof(DateTime))
                    {
                        p_ef.Worksheets[0].Columns[i].Style.NumberFormat = "dd/MM/yyyy";//#,#
                    }
                }
                //tạo style cho cell
                CellStyle cs = new CellStyle(p_ef);
                cs.Borders.SetBorders(MultipleBorders.Top | MultipleBorders.Right |
                MultipleBorders.Left | MultipleBorders.Bottom, System.Drawing.Color.Black, LineStyle.Thin);
                //duyệt row
                for (int i = 5; i < _new_dt.Rows.Count + 5; i++)
                {
                    //duyệt cột
                    for (int j = 0; j < _new_dt.Columns.Count; j++)
                    {
                        p_ef.Worksheets[0].Cells[i, j].Style = cs;
                    }
                }
                if (p_dt.Rows.Count - _new_dt.Rows.Count > 0)
                {
                    _sheet++;
                    DataTable _dt1 = p_dt.Clone();
                    _dt1.TableName = NameReport + "_sheet" + (_sheet + 1).ToString();

                    for (int i = _new_dt.Rows.Count; i < p_dt.Rows.Count; i++)
                    {
                        DataRow _dr = (DataRow)p_dt.Rows[i];
                        _dt1.Rows.Add(_dr.ItemArray);
                    }

                    ExportDataSet65k(ref p_ef, _dt1, 0, 64999, NameReport, _sheet, p_dic_format);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

       
        public static int ExportExcel(FlexCel.Report.FlexCelReport flcReport, string pathFileSource, string c_fileExport, ref string messageResult)
        {
            try
            {
                flcReport.DeleteEmptyRanges = false;
                var xlsMemoryStream = new MemoryStream();
                var pathSource = pathFileSource.Replace("//", "\\");

                // check xem file co ton tai trong duong dan ko?
                if (!System.IO.File.Exists(pathSource))
                {
                    messageResult = "Không tồn tại file mẫu trong thư mục mẫu!";
                    return -3; // không có file mẫu
                }

                // check file co dang mo hay ko
                using (var templateStream = new FileStream(pathSource, FileMode.Open))
                {
                    flcReport.Run(templateStream, xlsMemoryStream);

                    xlsMemoryStream.Position = 0;
                    var bytes = new byte[xlsMemoryStream.Length];
                    xlsMemoryStream.Read(bytes, 0, (int)xlsMemoryStream.Length);
                    using (var outStream = new FileStream(c_fileExport, FileMode.Create))
                    {
                        outStream.Write(bytes, 0, bytes.Length);
                        outStream.Close();
                        xlsMemoryStream.Close();
                        templateStream.Close();
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
             
                messageResult = "Lỗi tạo file báo cáo !";
                return -1;
            }
        }

        public static int ExportExcel_Stream(FlexCel.Report.FlexCelReport flcReport, string pathFileSource, string c_fileExport, ref string messageResult, ref string file64)
        {
            try
            {
                flcReport.DeleteEmptyRanges = false;
                var xlsMemoryStream = new MemoryStream();
                var pathSource = pathFileSource.Replace("//", "\\");

                // check xem file co ton tai trong duong dan ko?
                if (!System.IO.File.Exists(pathSource))
                {
                    messageResult = "Không tồn tại file mẫu trong thư mục mẫu!";
                    return -3; // không có file mẫu
                }

                // check file co dang mo hay ko
                using (var templateStream = new FileStream(pathSource, FileMode.Open))
                {
                    flcReport.Run(templateStream, xlsMemoryStream);

                    xlsMemoryStream.Position = 0;
                    var bytes = new byte[xlsMemoryStream.Length];
                    xlsMemoryStream.Read(bytes, 0, (int)xlsMemoryStream.Length);
                    using (var outStream = new FileStream(c_fileExport, FileMode.Create))
                    {
                        outStream.Write(bytes, 0, bytes.Length);
                        outStream.Close();
                        xlsMemoryStream.Close();
                        templateStream.Close();

                        // convert file thành base 64
                        Byte[] bytes_content = System.IO.File.ReadAllBytes(c_fileExport);
                        file64 = Convert.ToBase64String(bytes);

                        // xóa file đi
                        System.IO.File.Delete(c_fileExport);

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
              
                messageResult = "Lỗi tạo file báo cáo !";
                return -1;
            }
        }

        public static void SetValueExportByDataTable(ref FlexCel.Report.FlexCelReport flcReport, DataSet v_ds)
        {
            try
            {
                flcReport.AddTable(v_ds);
            }
            catch (Exception ex)
            {
                
            }
        }
        public static void SetValueExportByString(ref FlexCel.Report.FlexCelReport flcReport, string paramName, object value)
        {
            try
            {
                flcReport.SetValue(paramName, value);
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
