﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STXtoSQL.Log;
using STXtoSQL.DataAccess;
using STXtoSQL.Models;

namespace STXtoSQL_IPTJPC_NET
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogWrite("MSG", "Start: " + DateTime.Now.ToString());

            // Declare and defaults
            int odbcCnt = 0;
            int insertCnt = 0;
            int importCnt = 0;

            #region FromSTRATIX
            ODBCData objODBC = new ODBCData();

            List<IPTJPC> lstIPTJPC = new List<IPTJPC>();

            // Get data from Straix
            try
            {
                lstIPTJPC = objODBC.Get_IPTJPC();
            }
            catch (Exception ex)
            {
                Logger.LogWrite("EXC", ex);
                Logger.LogWrite("MSG", "Return");
                return;
            }
            #endregion

            #region ToSQL
            SQLData objSQL = new SQLData();

            // Only work in SQL database, if records were retreived from Stratix
            if (lstIPTJPC.Count != 0)
            {
                odbcCnt = lstIPTJPC.Count;

                // Put Stratix data in lstIPTJPC into IMPORT IPTJPC table
                try
                {
                    importCnt = objSQL.Write_IPTJPC_IMPORT(lstIPTJPC);
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                // Call SP to put IMPORT IPTJPC table data into WIP IPTJPC table
                try
                {
                    insertCnt = objSQL.Write_IMPORT_to_IPTJPC();
                }
                catch (Exception ex)
                {
                    Logger.LogWrite("EXC", ex);
                    Logger.LogWrite("MSG", "Return");
                    return;
                }

                Logger.LogWrite("MSG", "ODBC/IMPORT/INSERT=" + odbcCnt.ToString() + ":" + importCnt.ToString() + ":" + insertCnt.ToString());
            }
            else
                Logger.LogWrite("MSG", "No data");

            Logger.LogWrite("MSG", "End: " + DateTime.Now.ToString());
            #endregion

            // Testing
            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }
    }
}
