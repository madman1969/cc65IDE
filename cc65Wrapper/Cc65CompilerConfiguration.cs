﻿using System;

namespace cc65Wrapper
{
    public class Cc65CompilerConfiguration
    {
        #region Constants

        const string CC65_HOME = "CC65_HOME";
        const string CC65_INC = "CC65_INC";
        const string LD65_CFG = "LD65_CFG";
        const string LD65_LIB = "LD65_LIB";
        const string MAKE_HOME = "MAKE_HOME";

        #endregion 

        #region Fields and properties 

        public string cc65Home { get; set; }
        public string cc65Include { get; set; }
        public string ld65Cfg { get; set; }
        public string ld65Lib { get; set; }
        public string makeHome { get; set; }

        #endregion

        #region Class Constructor 

        public Cc65CompilerConfiguration()
        {
            // Try to read env vars ...
            var envVars = Environment.GetEnvironmentVariables();

            // Use env var settings, if present ...
            cc65Home = envVars.Contains(CC65_HOME) ? envVars[CC65_HOME].ToString() : string.Empty;
            cc65Include = envVars.Contains(CC65_INC) ? envVars[CC65_INC].ToString() : string.Empty;
            ld65Cfg = envVars.Contains(LD65_CFG) ? envVars[LD65_CFG].ToString() : string.Empty;
            ld65Lib = envVars.Contains(LD65_LIB) ? envVars[LD65_LIB].ToString() : string.Empty;
            makeHome = envVars.Contains(MAKE_HOME) ? envVars[MAKE_HOME].ToString() : string.Empty;
        }

        #endregion

        /// <summary>
        /// Saves the current CC65 configuration back to the environment variables
        /// </summary>
        /// <returns></returns>
        public bool SaveConfiguration()
        {
            var result = false;

            try
            {
                // Try to save the settings ...
                Environment.SetEnvironmentVariable(CC65_HOME, cc65Home);
                Environment.SetEnvironmentVariable(CC65_INC, cc65Include);
                Environment.SetEnvironmentVariable(LD65_CFG, ld65Cfg);
                Environment.SetEnvironmentVariable(LD65_LIB, ld65Lib);
                Environment.SetEnvironmentVariable(MAKE_HOME, makeHome);
                
                // If got here then must be successful ...
                result = true;
            }
            catch (Exception ex)
            {
                // Oops ...
            }

            return result;
        }
    }
}