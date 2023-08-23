using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace Common.Utilities
{
    public static class CommonUtilities
    {

        private const string _fileDirectory = "Enter Path of Where Excel File is Located that will be used for Editing";

        public static T DeserializeJsontoObject<T>(string jsonToDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(jsonToDeserialize);
        }

        public static string SerializeObjecttoJson(this object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
        }

        public static string GetExceptionString(ref Exception oE)
        {
            System.Text.StringBuilder oSB = new System.Text.StringBuilder(300);
            oSB.Append("Error type of System.Exception occured:\r\n");
            oSB.Append("InnerException:\t" + oE.InnerException + "\r\n");
            oSB.Append("Message:\t" + oE.Message + "\r\n");
            oSB.Append("Source:\t" + oE.Source + "\r\n");
            oSB.Append("TargetSite:\t" + oE.TargetSite + "\r\n");
            oSB.Append("Stack:\t" + oE.StackTrace + "\r\n");
            return oSB.ToString();
        }

        public static bool IsValueInt32(string valueToCheck)
        {
            Int32 value;
            if (Int32.TryParse(valueToCheck, out value))
            { return true; }
            else
            { return false; }
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static void deleteTabFromExcelFile()
        {              
        Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.DisplayAlerts = false;
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(_fileDirectory, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            Excel.Worksheet worksheet = (Excel.Worksheet)xlWorkBook.Worksheets[17];          
            worksheet.Delete();
            Thread.Sleep(3000);
            xlWorkBook.Save();
            xlWorkBook.Close();
            xlApp.Quit();
            CloseExcel(xlApp);
        }

        private static void CloseExcel(Excel.Application ExcelApplication = null)
        {
            if (ExcelApplication != null)
            {
                ExcelApplication.Workbooks.Close();
                ExcelApplication.Quit();
            }

            System.Diagnostics.Process[] PROC = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            foreach (System.Diagnostics.Process PK in PROC)
            {
                if (PK.MainWindowTitle.Length == 0) { PK.Kill(); }
            }
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                Console.WriteLine("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
