namespace NetworkEditor.Utils
{
    using System;
    using System.IO;
    using System.Reflection;
    using cohtml.Net;

    /// <summary>
    /// Utilities for loading and parsing UI related files (JS).
    /// </summary>
    internal static class UIFileUtils
    {
        /// <summary>
        /// Executes JavaScript in the given View.
        /// </summary>
        /// <param name="view"><see cref="View"/> to execute in.</param>
        /// <param name="script">Script to execute.</param>
        internal static void ExecuteScript(View view, string script)
        {
            // Null check.
            if (!string.IsNullOrEmpty(script))
            {
                view?.ExecuteScript(script);
            }
        }

        /// <summary>
        /// Load and execute JavaScript from a UI file.
        /// </summary>>
        /// <param name="fileName">UI file name to read.</param>
        /// <returns>JavaScript as <see cref="string"/> (<c>null</c> if empty or error).</returns>
        internal static string ReadJS(string fileName)
        {
            try
            {
                // Attempt to read file.
                string js = ReadUIFile(fileName);

                // Don't do anything if file wasn't read.
                if (!string.IsNullOrEmpty(js))
                {
                    // Return JavaScript code with HTML embedded.
                    return js;
                }
            }
            catch (Exception e)
            {
                Mod.Instance.Log.Error(e, $" exception reading JS file {fileName}");
            }

            // If we got here, something went wrong.; return null.
            return null;
        }

        /// <summary>
        /// Reads a UI text file.
        /// </summary>
        /// <param name="fileName">UI file name to read.</param>
        /// <returns>File contents (<c>null</c> if none or error).</returns>
        private static string ReadUIFile(string fileName)
        {
            try
            {
                // Check that file exists.
                using Stream embeddedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                using System.IO.StreamReader reader = new(embeddedStream);
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Mod.Instance.Log.Error(e, $" exception reading UI file {fileName}");
            }

            // If we got here, something went wrong.
            return null;
        }
    }
}
