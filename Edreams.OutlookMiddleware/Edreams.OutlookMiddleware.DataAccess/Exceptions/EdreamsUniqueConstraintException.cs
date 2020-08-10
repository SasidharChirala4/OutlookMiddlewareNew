using System;

namespace Edreams.OutlookMiddleware.DataAccess.Exceptions
{
    /// <summary>
    /// Represents DataAccess errors that occur during e-DReaMS application execution.
    /// </summary>
    /// <seealso cref="EdreamsDataAccessException" />
    public class EdreamsUniqueConstraintException : EdreamsDataAccessException
    {
        /// <summary>
        /// Gets or sets the data-source.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdreamsUniqueConstraintException"/> class.
        /// </summary>
        /// <param name="datasource">The data-source.</param>
        /// <param name="database">The database.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <param name="innerException">The inner exception.</param>
        public EdreamsUniqueConstraintException(string datasource, string database, string table, string column, Exception innerException) : base(innerException)
        {
            DataSource = datasource;
            Database = database;
            Table = table;
            Column = column;
        }
    }
}