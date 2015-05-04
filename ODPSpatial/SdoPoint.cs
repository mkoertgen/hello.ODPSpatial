using Oracle.DataAccess.Types;

namespace ODPSpatial
{
    /// <summary>
    /// This is a user defined type to map the SQL/MM (3) MDSYS.SDO_POINT_TYPE
    /// to a .NET class
    /// </summary>
    [OracleCustomTypeMapping("MDSYS.SDO_POINT_TYPE")]
    public sealed class SdoPoint : OracleCustomTypeBase<SdoPoint>
    {
        #region Documentation

        /* To verifiy the mapping, execute the SQL-statement "describe MDSYS.SDO_POINT_TYPE":
         * 
         * user type definition                                                           
         * ------------------------------------------------------------------------------ 
         * TYPE SDO_POINT_TYPE                                                            
         * 
         *      AS OBJECT (                                                                
         *          X       NUMBER,                                                         
         *          Y       NUMBER,                                                         
         *          Z       NUMBER)                                                         
         */

        #endregion

        #region Fields

        private decimal? _x;
        private decimal? _y;
        private decimal? _z;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the X ordinate.
        /// </summary>
        /// <value>
        /// The X ordinat.
        /// </value>
        [OracleObjectMapping(0)] //"X")] // NOTE: field-id is faster than name!
        public decimal? X { get { return _x; } set { _x = value; } }

        /// <summary>
        /// Gets or sets the X ordinate as <see cref="double"/>.
        /// </summary>
        /// <value>
        /// The X ordinate as <see cref="double"/>..
        /// </value>
        public double? Xd { get { return System.Convert.ToDouble(_x); } set { _x = System.Convert.ToDecimal(value); } }

        /// <summary>
        /// Gets or sets the Y ordinate.
        /// </summary>
        /// <value>
        /// The Y ordinate.
        /// </value>
        [OracleObjectMapping(1)] //"Y")]
        public decimal? Y { get { return _y; } set { _y = value; } }


        /// <summary>
        /// Gets or sets the Y ordinate as <see cref="double"/>.
        /// </summary>
        /// <value>
        /// The Y ordinate as <see cref="double"/>.
        /// </value>
        public double? Yd { get { return System.Convert.ToDouble(_y); } set { _y = System.Convert.ToDecimal(value); } }

        /// <summary>
        /// Gets or sets the Z ordinate.
        /// </summary>
        /// <value>
        /// The Z ordinate.
        /// </value>
        [OracleObjectMapping(2)] //"Z")]
        public decimal? Z { get { return _z; } set { _z = value; } }

        /// <summary>
        /// Gets or sets the Z ordinate as <see cref="double"/>.
        /// </summary>
        /// <value>
        /// The Z ordinate as <see cref="double"/>.
        /// </value>
        public double? Zd { get { return System.Convert.ToDouble(_z); } set { _z = System.Convert.ToDecimal(value); } }

        #endregion

        #region Methods

        /// <summary>
        /// Maps from custom object.
        /// </summary>
        public override void MapFromCustomObject()
        {
            SetValue(0, _x); //"X", x);
            SetValue(1, _y); //"Y", y);
            SetValue(2, _z); //"Z", z);
        }

        /// <summary>
        /// Maps to custom object.
        /// </summary>
        public override void MapToCustomObject()
        {
            X = GetValue<decimal?>(0); //"X");
            Y = GetValue<decimal?>(1); //"Y");
            Z = GetValue<decimal?>(2); //"Z");
        }

        #endregion
    }
}
