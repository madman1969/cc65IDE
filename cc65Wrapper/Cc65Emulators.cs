using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cc65Wrapper
{
    public class Cc65Emulators
    {
        #region Fields and properties

        /// <summary>
        /// Gets or sets the C64 path.
        /// </summary>
        /// <value>
        /// The C64 path.
        /// </value>
        public string c64Path { get; set; }

        /// <summary>
        /// Gets or sets the C128 path.
        /// </summary>
        /// <value>
        /// The C128 path.
        /// </value>
        public string c128Path { get; set; }

        /// <summary>
        /// Gets or sets the PET path.
        /// </summary>
        /// <value>
        /// The pet path.
        /// </value>
        public string petPath { get; set; }

        #endregion

        #region Class Constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        public Cc65Emulators()
        {
            c64Path = string.Empty;
            c128Path = string.Empty;
            petPath = string.Empty;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves JSON representation of a project
        /// </summary>
        /// <returns></returns>
        public string AsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Convert JSON representation into project instance
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static Cc65Emulators FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<Cc65Emulators>(Json);
        }

        #endregion
    }
}
