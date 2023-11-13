using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;


namespace TextAlignment
{
    public class TextAlignment
    {
        [CommandMethod("TATask")]
        public static void TextAlign()
        {
            //Get the current document and database
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor edt = doc.Editor;

            //Strat transaction
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    edt.WriteMessage("Text Alignment! ");
                    //open the blocktable for reading
                    BlockTable bt;
                    bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    //open the block table record modelspace for writing
                    BlockTableRecord btr;
                    btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    //max length of a string of Mtext
                    int maxLength = 90;

                    //Get first user input/line (userInput1)
                    edt.WriteMessage("Enter first value");
                    string userInput1 = GetUserInput().ToUpper();
                    List<string> userInputStrings = SplitString(userInput1, maxLength);

                    //get current text height
                    double txtHeight = HostApplicationServices.WorkingDatabase.Textsize;


                    //FIRST spacing (15 units), set all other spacings (10 units)
                    int insertionPointDist = -15;
                    int spacingPointDist = 10;
                    Point3d startPt = new Point3d(0, insertionPointDist, 0);

                    //To save last text location
                    double lastMTextLocation = 0;

                    //first user entry Mtexts location depending on max allowed length
                    for (int index = 0; index < userInputStrings.Count; index++)
                    {
                        MText mText = new MText();
                        mText.Contents = userInputStrings[index];


                        //if text is first, then spacing from above is 15 units
                        if (index == 0)
                        {
                            mText.Location = startPt;

                            //if it is last string in a List, save last text location
                            if (index == userInputStrings.Count - 1)
                            {
                                lastMTextLocation = insertionPointDist;
                            }
                        }
                        else
                        {
                            //else spacing is 10 points
                            double insertionPoint = insertionPointDist - index * (txtHeight + spacingPointDist);
                            Point3d insertionPt = new Point3d(0, insertionPoint, 0);
                            mText.Location = insertionPt;

                            //if it is last string in a List, save last text location
                            if (index == userInputStrings.Count - 1)
                            {
                                lastMTextLocation = insertionPoint;
                            }
                        }


                        btr.AppendEntity(mText);
                        trans.AddNewlyCreatedDBObject(mText, true);

                    }



                    //SECOND user entry
                    edt.WriteMessage("Enter second value");
                    string userInput2 = GetUserInput().ToUpper();
                    List<string> userInputStrings2 = SplitString(userInput2, maxLength);

                    for (int index = 0; index < userInputStrings2.Count; index++)
                    {
                        MText mText = new MText();
                        mText.Contents = userInputStrings2[index];

                        double insertionPoint = lastMTextLocation - ((index + 1) * (txtHeight + spacingPointDist));
                        Point3d insertionPt = new Point3d(0, insertionPoint, 0);
                        mText.Location = insertionPt;

                        //if it is last string in a List, save last text location
                        if (index == userInputStrings2.Count - 1)
                        {
                            lastMTextLocation = insertionPoint;
                        }

                        btr.AppendEntity(mText);
                        trans.AddNewlyCreatedDBObject(mText, true);
                    }


                    //THIRD user entry
                    edt.WriteMessage("Enter third value");
                    string userInput3 = GetUserInput().ToUpper();
                    List<string> userInputStrings3 = SplitString(userInput3, maxLength);

                    for (int index = 0; index < userInputStrings3.Count; index++)
                    {
                        MText mText = new MText();
                        mText.Contents = userInputStrings3[index];

                        double insertionPoint = lastMTextLocation - ((index + 1) * (txtHeight + spacingPointDist));
                        Point3d insertionPt = new Point3d(0, insertionPoint, 0);
                        mText.Location = insertionPt;

                        //if it is last string in a List, save last text location
                        if (index == userInputStrings3.Count - 1)
                        {
                            lastMTextLocation = insertionPoint;
                        }

                        btr.AppendEntity(mText);
                        trans.AddNewlyCreatedDBObject(mText, true);
                    }


                    trans.Commit();

                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage("Error: " + ex.Message);
                    trans.Abort();
                }
            }
        }


        public static string GetUserInput()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor edt = doc.Editor; //all user interactions from here
            string userInput = "SUPERA 83 PASSIVE ENTRY DOOR (MAX H 92\") W/ ADA MAGNETIC THRESHOLD, IN-SWING: TURN (IN-SWING DOORS)";

            //Promt the users using PromptStringOptions
            PromptStringOptions prompt = new PromptStringOptions("");
            prompt.AllowSpaces = true;

            //Get results of the user input using PromptResult
            PromptResult result = edt.GetString(prompt);
            if (result.Status == PromptStatus.OK & result.StringResult != "")
            {
                userInput = result.StringResult;
            }
            else
            {
                edt.WriteMessage("No value entered. Creating default text...");
            }
            return userInput;
        }


        static List<string> SplitString(string input, int maxLength)
        {
            List<string> result = new List<string>();

            while (input.Length > 0)
            {
                // If the remaining string is shorter than or equal to maxLength, add it as is
                if (input.Length <= maxLength)
                {
                    result.Add(input);
                    break;
                }

                // Find the last space before maxLength
                int lastSpace = input.LastIndexOf(' ', maxLength);
                int length = (lastSpace > 0) ? lastSpace : maxLength;

                // Add the substring up to the last space (or maxLength if no space found)
                result.Add(input.Substring(0, length));

                // Remove the processed part of the string, plus any leading spaces
                input = input.Substring(length).TrimStart();
            }

            return result;
        }


    }
}