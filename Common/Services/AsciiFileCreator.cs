using Common.Models;
using Common.Utilities;
using Common.Enumerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Cells;
using System.Threading;
using Common.TeamCityModels;

namespace Common.Services
{
    public static class AsciiFileCreator
    {
        private const string TIME_STAMP_FORMAT = "yyyy-MMM-dd.hh-mm-ss.tt";  
        private static string _fileDirectory = "Enter Path of Where Excel File is Located that will be used for Editing";

        public static void UpdateExcelFile(List<CommitTargetProcessItem> commits, EnumOutputFileType fileType, int sheetNumber, int sprintNumber)
        {
            string notFound = "--";
            Workbook workbook = new Workbook(_fileDirectory + ".xlsx");
            var sheet1 = workbook.Worksheets[sheetNumber];
            Cells sheet1Cells = sheet1.Cells;
            Aspose.Cells.Cell cell;
            for (int i = 5; i < 2000; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    cell = sheet1.Cells[i, j];
                    cell.PutValue("");
                }
            }
            int rowCounter = 4;
            foreach (CommitTargetProcessItem commit in commits)
                {
                switch (fileType)
                    {
                    case EnumOutputFileType.MissingUS:
                        int propertyCounter = 0;
                        ++rowCounter;
                        string[] entityProperties = new string[] { commit.Commit.Comment, (commit.IsInDevelopment ? "Yes" : "No"), (commit.Commit.Author.Email.Substring(0, commit.Commit.Author.Email.IndexOf("@"))).Replace(".", " ").ToLower(), 
                            commit.CommitId, commit.Commit.RemoteUrl };

                        cell = sheet1.Cells[rowCounter, 1];
                        cell.PutValue(commit.DeveloperTeam);
                        cell = sheet1.Cells[rowCounter, 2];
                        cell.PutValue(sprintNumber);

                        for (int i = 3; i <= entityProperties.Length + 2; i++)
                        {
                            cell = sheet1.Cells[rowCounter, i];
                            cell.PutValue(entityProperties[propertyCounter]);
                            ++propertyCounter;
                        }
                        break;
                        case EnumOutputFileType.WrongTeamSprint:
                        propertyCounter = 0;
                        ++rowCounter;
                        entityProperties = new string[] { (commit.UserStory == null ? notFound : commit.UserStory.ResourceType), (commit.UserStory == null ? notFound : commit.UserStory.State.Name),
                            (commit.IsInDevelopment ? "Yes" : "No"), (commit.UserStory == null ? notFound : commit.UserStory.TeamIteration.Name),
                            (commit.Commit.Author.Email.Substring(0, commit.Commit.Author.Email.IndexOf("@"))).Replace(".", " ").ToLower(), commit.Commit.Comment};

                        cell = sheet1.Cells[rowCounter, 1];
                        cell.PutValue(sprintNumber);
                        cell = sheet1.Cells[rowCounter, 2];
                        cell.PutValue((commit.CommitUSId == null ? notFound : commit.CommitUSId.ToString()));

                        for (int i = 3; i <= entityProperties.Length + 2; i++)
                        {
                            cell = sheet1.Cells[rowCounter, i];
                            cell.PutValue(entityProperties[propertyCounter]);
                            ++propertyCounter;
                        }
                        break;
                        case EnumOutputFileType.CommitOnDoneTpEntity:
                        propertyCounter = 0;
                        ++rowCounter;
                        entityProperties = new string[] { (commit.UserStory == null ? notFound : commit.UserStory.ResourceType), (commit.UserStory == null ? notFound : commit.UserStory.State.Name),
                            commit.UserStoryHistory.Date.ToString(),(commit.IsInDevelopment ? "Yes" : "No"), (commit.UserStory == null ? notFound : commit.UserStory.TeamIteration.Name),
                            (commit.Commit.Author.Email.Substring(0, commit.Commit.Author.Email.IndexOf("@"))).Replace(".", " ").ToLower(), commit.Commit.Comment, commit.Commit.Committer.Date.ToString()};

                        cell = sheet1.Cells[rowCounter, 1];
                        cell.PutValue(sprintNumber);
                        cell = sheet1.Cells[rowCounter, 2];
                        cell.PutValue((commit.CommitUSId == null ? notFound : commit.CommitUSId.ToString()));

                        for (int i = 3; i <= entityProperties.Length + 2; i++)
                        {
                            cell = sheet1.Cells[rowCounter, i];
                            cell.PutValue(entityProperties[propertyCounter]);
                            ++propertyCounter;
                        }
                        break;
                    }
                }
            workbook.CalculateFormula();
            workbook.Save(_fileDirectory + ".xlsx", SaveFormat.Xlsx);
        }
        
        public static void UpdateExcelFile(List<TargetProcessItem> entities,int sheetNumber, int sprintNumber)
        {
            Workbook workbook = new Workbook(_fileDirectory + ".xlsx");
            var sheet1 = workbook.Worksheets[sheetNumber];
            Cells sheet1Cells = sheet1.Cells;
            Aspose.Cells.Cell cell;
            for (int i =5; i < 2000; i++)
            {
                for(int j = 1; j < 15; j++)
                {
                    cell = sheet1.Cells[i, j];
                    cell.PutValue("");
                }
            }
            int rowCounter = 4;
            foreach (TargetProcessItem entity in entities)
            {
                int propertyCounter = 0;
                ++rowCounter;
                string[] entityProperties = new string[] {entity.ResourceType, entity.TeamIteration.Name, entity.State.Name };

                cell = sheet1.Cells[rowCounter, 1];
                cell.PutValue(sprintNumber);
                cell = sheet1.Cells[rowCounter, 2];
                cell.PutValue(entity.Id);

                for (int i = 3; i <= entityProperties.Length+2; i++)
                {
                    cell = sheet1.Cells[rowCounter, i];
                    cell.PutValue(entityProperties[propertyCounter]);
                    ++propertyCounter;
                }
            }
            workbook.CalculateFormula();
            workbook.Save(_fileDirectory + ".xlsx", SaveFormat.Xlsx);
        }

        public static void UpdateExcelFile(List<TargetProcessItemStateHistory> entities, int sheetNumber, int sprintNumber)
        {
            Workbook workbook = new Workbook(_fileDirectory + ".xlsx");
            var sheet1 = workbook.Worksheets[sheetNumber];
            Cells sheet1Cells = sheet1.Cells;
            Aspose.Cells.Cell cell;
            for (int i = 5; i < 2000; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    cell = sheet1.Cells[i, j];
                    cell.PutValue("");
                }
            }
            int rowCounter = 4;
            foreach (TargetProcessItemStateHistory entityStateHistory in entities)
            {
                int propertyCounter = 0;
                ++rowCounter;
                string[] entityProperties = new string[] {entityStateHistory.ResourceType, entityStateHistory.TeamIteration.Name,
                   entityStateHistory.Date.ToString("MM/dd/yyyy hh:mm:ss tt"), entityStateHistory.EntityState.Name,entityStateHistory.UserStory.Name};

                cell = sheet1.Cells[rowCounter, 1];
                cell.PutValue(sprintNumber);
                cell = sheet1.Cells[rowCounter, 2];
                cell.PutValue(entityStateHistory.UserStory.Id);

                for (int i = 3; i <= entityProperties.Length + 2; i++)
                {
                    cell = sheet1.Cells[rowCounter, i];
                    cell.PutValue(entityProperties[propertyCounter]);
                    ++propertyCounter;
                }
            }
            workbook.CalculateFormula();
            workbook.Save(_fileDirectory + ".xlsx", SaveFormat.Xlsx);
        }

        public static void UpdateExcelFileForTasks(List<TargetProcessItemStateHistory> entities, int sheetNumber, int sprintNumber)
        {
            Workbook workbook = new Workbook(_fileDirectory + ".xlsx");
            var sheet1 = workbook.Worksheets[sheetNumber];
            Cells sheet1Cells = sheet1.Cells;
            Aspose.Cells.Cell cell;
            for (int i = 5; i < 2000; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    cell = sheet1.Cells[i, j];
                    cell.PutValue("");
                }
            }
            int rowCounter = 4;
            foreach (TargetProcessItemStateHistory entityStateHistory in entities)
            {
                int propertyCounter = 0;
                ++rowCounter;
                string[] entityProperties = new string[] {entityStateHistory.ResourceType, entityStateHistory.TeamIteration.Name,entityStateHistory.UserStory.Name};

                cell = sheet1.Cells[rowCounter, 1];
                cell.PutValue(sprintNumber);
                cell = sheet1.Cells[rowCounter, 2];
                cell.PutValue(entityStateHistory.UserStory.Id);

                for (int i = 3; i <= entityProperties.Length + 2; i++)
                {
                    cell = sheet1.Cells[rowCounter, i];
                    cell.PutValue(entityProperties[propertyCounter]);
                    ++propertyCounter;
                }
            }
            workbook.CalculateFormula();
            workbook.Save(_fileDirectory + ".xlsx", SaveFormat.Xlsx);
        }

        public static void UpdateExcelFileForTasks(List<TeamCityBuilds> entities, int sheetNumber, int sprintNumber)
        {
            Workbook workbook = new Workbook(_fileDirectory + ".xlsx");
            var sheet1 = workbook.Worksheets[sheetNumber];
            Cells sheet1Cells = sheet1.Cells;
            Aspose.Cells.Cell cell;
            for (int i = 5; i < 2000; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    cell = sheet1.Cells[i, j];
                    cell.PutValue("");
                }
            }
            int rowCounter = 4;
            foreach (TeamCityBuilds builds in entities)
            {
                ++rowCounter;
                string[] entityProperties = new string[] { builds.buildType.Name };

                cell = sheet1.Cells[rowCounter, 1];
                cell.PutValue(sprintNumber);
                cell = sheet1.Cells[rowCounter, 2];
                cell.PutValue(builds.buildType.Name);                
            }
            workbook.CalculateFormula();
            workbook.Save(_fileDirectory + ".xlsx", SaveFormat.Xlsx);
        }        

        public static void SetCellStyleBold(Aspose.Cells.Cell cell)
        {            
            Style style = cell.GetStyle();
            style.Font.IsBold = true;
            cell.SetStyle(style);
        }        
    }
}
