using System;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;

namespace ODPSpatial
{
    /// <summary>
    /// Abstract base class for converting between a Custom CLR Type and an Oracle Object or Collection Type.
    /// </summary>
    /// <typeparam name="T">The Clr type to be mapped to Oracle.</typeparam>
    public abstract class OracleCustomTypeBase<T>
        : INullable
        , IOracleCustomType, IOracleCustomTypeFactory
        where T : OracleCustomTypeBase<T>, new()
    {
        private const string ErrorMessageTemplate = "Error converting Oracle User Defined Type to .Net Type {0}, oracle column is null, failed to map to . NET valuetype, column {1} of value type {2}";

        private OracleConnection _connection;
        private IntPtr _pUdt;
        private bool _isNull;

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsNull { get { return _isNull; } }

        /// <summary>
        /// Gets the static null instance.
        /// </summary>
        public static T Null { get { var t = new T {_isNull = true}; return t; } }

        /// <summary>
        /// Creates the custom object.
        /// </summary>
        /// <returns></returns>
        public IOracleCustomType CreateObject() { return new T(); }

        /// <summary>
        /// Sets the connection and pointer.
        /// </summary>
        /// <param name="connection">The oracle connection.</param>
        /// <param name="pUdt">An opaque pointer to the Oracle Object or Collection to be created.</param>
        protected void SetConnectionAndPointer(OracleConnection connection, IntPtr pUdt)
        {
            _connection = connection;
            _pUdt = pUdt;
        }

        /// <summary>
        /// Maps from the custom object to the oracle object.
        /// </summary>
        public abstract void MapFromCustomObject();

        /// <summary>
        /// Maps from oracle opbject to custom object.
        /// </summary>
        public abstract void MapToCustomObject();

        /// <summary>
        /// Returns the values that set the Oracle Object attributes.
        /// </summary>
        /// <param name="con">An OracleConnection instance.</param>
        /// <param name="pUdt">An opaque pointer to the Oracle Object or Collection to be created.</param>
        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            SetConnectionAndPointer(con, pUdt);
            MapFromCustomObject();
        }

        /// <summary>
        /// Provides the Oracle Object with the attribute values to set on the custom type.
        /// </summary>
        /// <param name="con">An OracleConnection instance.</param>
        /// <param name="pUdt">An opaque pointer to the Oracle Object or Collection to be created.</param>
        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            SetConnectionAndPointer(con, pUdt);
            MapToCustomObject();
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="oracleColumnName">Name of the oracle column.</param>
        /// <param name="value">The value.</param>
        protected void SetValue(string oracleColumnName, object value)
        {
            if (value != null)
                OracleUdt.SetValue(_connection, _pUdt, oracleColumnName, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="oracleColumnId">The oracle column id.</param>
        /// <param name="value">The value.</param>
        protected void SetValue(int oracleColumnId, object value)
        {
            if (value != null)
                OracleUdt.SetValue(_connection, _pUdt, oracleColumnId, value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="TColumn">The value type.</typeparam>
        /// <param name="oracleColumnName">Name of the oracle column.</param>
        /// <returns></returns>
        protected TColumn GetValue<TColumn>(string oracleColumnName)
        {
            if (OracleUdt.IsDBNull(_connection, _pUdt, oracleColumnName))
            {
                if (default(TColumn) is ValueType)
                    throw new InvalidCastException(String.Format(ErrorMessageTemplate, typeof(T), oracleColumnName, typeof(TColumn)));
                return default(TColumn);
            }
            return (TColumn)OracleUdt.GetValue(_connection, _pUdt, oracleColumnName);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="TColumn">The value type.</typeparam>
        /// <param name="oracleColumnId">The oracle columnid.</param>
        /// <returns></returns>
        protected TColumn GetValue<TColumn>(int oracleColumnId)
        {
            if (!OracleUdt.IsDBNull(_connection, _pUdt, oracleColumnId))
                return (TColumn) OracleUdt.GetValue(_connection, _pUdt, oracleColumnId);
            if (default(TColumn) is ValueType)
                throw new InvalidCastException(String.Format(ErrorMessageTemplate, typeof (T), oracleColumnId, typeof (TColumn)));
            return default(TColumn);
        }
    }
}
