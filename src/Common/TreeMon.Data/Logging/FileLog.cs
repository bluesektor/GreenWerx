// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;

namespace TreeMon.Data.Logging
{
    public class FileLog
    {
        #region Fields

        /// <summary>
        /// Mutex
        /// </summary>
        private static Dictionary<String, Mutex> _Mutex = new Dictionary<String, Mutex>();

        /// <summary>
        /// Filename
        /// </summary>
        private String _filename;

        #endregion Fields


        #region Constructors

        /// <summary>
        /// Instantiate an object of type LogFile
        /// </summary>
        /// <param name="filename">Name of the log file</param>
        public void LogFile(String pathToFile)
        {
            this._filename = pathToFile;
            try
            {
                if (!_Mutex.ContainsKey(pathToFile))
                {
                    _Mutex.Add(pathToFile, new Mutex());
                }
            }
            catch { }

        } 

        #endregion Constructors


        #region Private methods

        /// <summary>
        /// Waits the mutex.
        /// </summary>
        /// <returns>The exception if an exception occured; otherwise null</returns>
        private Exception WaitMutex()
        {
            Exception result = null;

            if (!_Mutex.ContainsKey(this._filename))
            {
                _Mutex.Add(this._filename, new Mutex());
            }

            try
            {
                _Mutex[this._filename].WaitOne();
            }
            catch (AbandonedMutexException)
            {
                // The mutex will still get aquired

            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        } 

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="message">The message.</param>
        private void WriteMessage(StreamWriter writer, String message)
        {
            if (message != null)
            {
                writer.Write(DateTime.UtcNow);
                writer.WriteLine(" " + message);
            }
        } 

        /// <summary>
        /// Writes the exception.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="ex">The ex.</param>
        private void WriteException(StreamWriter writer, Exception ex)
        {
            if (ex != null)
            {
                writer.Write(DateTime.UtcNow);
                writer.WriteLine(" Message: " + ex.Message);

                writer.Write(DateTime.UtcNow);
                writer.WriteLine(" StackTrace: " + ex.StackTrace);

                writer.Write(DateTime.UtcNow);
                writer.WriteLine(" Source: " + ex.Source);
            }
        } 


        /// <summary>
        /// Writes the command.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="command">The command.</param>
        private void WriteCommand(StreamWriter writer, IDbCommand command)
        {
            if (command != null)
            {
                writer.Write(DateTime.UtcNow);
                writer.WriteLine(" Query: " + command.CommandText);

                if (command.Parameters.Count > 0)
                {

                    writer.Write(DateTime.UtcNow);
                    writer.WriteLine(" Parameters: ");

                    foreach (IDataParameter parameter in command.Parameters)
                    {
                        writer.Write("\t" + parameter.ParameterName + ": " + parameter.Value);
                    }

                }
            }
        } 

        #endregion Private methods

        #region Public methods

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <returns>The exception if an exception occured; otherwise null</returns>
        public Exception Write(String message)
        {
            Exception result = null;

            result = this.WaitMutex();

            if (result != null)
            {
                return result;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Append, FileAccess.Write, FileShare.Read)))
                {
                    this.WriteMessage(writer, message);
                    writer.WriteLine();
                }
            }
            catch (Exception ex)
            {
                result = ex;
            }
            finally
            {
                _Mutex[this._filename].ReleaseMutex();
            }

            return result;
        } 

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="ex">Exception to write</param>
        /// <returns>The exception if an exception occured; otherwise null</returns>
        public Exception Write(String message, Exception ex)
        {
            Exception result = null;

            result = this.WaitMutex();

            if (result != null)
            {
                return result;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Append, FileAccess.Write, FileShare.Read)))
                {
                    this.WriteMessage(writer, message);

                    this.WriteException(writer, ex);

                    writer.WriteLine();
                }
            }
            catch (Exception lfex)
            {
                result = lfex;
            }
            finally
            {
                _Mutex[this._filename].ReleaseMutex();
            }

            return result;
        } 

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="command">The command.</param>
        /// <returns>
        /// The exception if an exception occured; otherwise null
        /// </returns>
        public Exception Write(String message, IDbCommand command)
        {
            Exception result = null;

            result = this.WaitMutex();

            if (result != null)
            {
                return result;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Append, FileAccess.Write, FileShare.Read)))
                {
                    this.WriteMessage(writer, message);

                    this.WriteCommand(writer, command);

                    writer.WriteLine();
                }
            }
            catch (Exception lfex)
            {
                result = lfex;
            }
            finally
            {
                _Mutex[this._filename].ReleaseMutex();
            }

            return result;
        } 

        /// <summary>
        /// Write a message in the log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="ex">Exception to write</param>
        /// <param name="command">The command.</param>
        /// <returns>
        /// The exception if an exception occured; otherwise null
        /// </returns>
        public Exception Write(String message, Exception ex, IDbCommand command)
        {
            Exception result = null;

            result = this.WaitMutex();

            if (result != null)
            {
                return result;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open(this._filename, FileMode.Append, FileAccess.Write, FileShare.Read)))
                {
                    this.WriteMessage(writer, message);

                    this.WriteException(writer, ex);

                    this.WriteCommand(writer, command);

                    writer.WriteLine();
                }
            }
            catch (Exception lfex)
            {
                result = lfex;
            }
            finally
            {
                _Mutex[this._filename].ReleaseMutex();
            }

            return result;
        }

        #endregion Public methods
    }
}
