using System;
using Oracle.DataAccess.Types;

namespace ODPSpatial
{
    /// <summary>
    /// This is a user defined type to map the Oracle Spatial/Locator type MDSYS.SDO_GEOMETRY
    /// to a .NET class
    /// </summary>
    [OracleCustomTypeMapping("MDSYS.SDO_GEOMETRY")]
    public class SdoGeometry : OracleCustomTypeBase<SdoGeometry>
    {
        #region Documentation

        /* To verifiy the mapping, execute the SQL-statement "describe MDSYS.SDO_GEOMETRY":
         * 
         * user type definition                                             
         * ---------------------------------------------------------------- 
         * TYPE SDO_GEOMETRY          AS OBJECT (                           
         *          SDO_GTYPE       NUMBER,                               
         *          SDO_SRID        NUMBER,                               
         *          SDO_POINT       SDO_POINT_TYPE,                       
         *          SDO_ELEM_INFO   SDO_ELEM_INFO_ARRAY,                  
         *          SDO_ORDINATES   SDO_ORDINATE_ARRAY,                   
         *          MEMBER FUNCTION  GET_GTYPE                            
         *          RETURN NUMBER DETERMINISTIC,                          
         *          MEMBER FUNCTION  GET_DIMS                             
         *          RETURN NUMBER DETERMINISTIC,                          
         *          MEMBER FUNCTION  GET_LRS_DIM                          
         *          RETURN NUMBER DETERMINISTIC)                          
         * 
         * ALTER TYPE SDO_GEOMETRY                                        
         * ADD MEMBER FUNCTION GET_WKB RETURN BLOB DETERMINISTIC,         
         * ADD MEMBER FUNCTION GET_WKT RETURN CLOB DETERMINISTIC,         
         * ADD MEMBER FUNCTION ST_CoordDim RETURN SMALLINT DETERMINISTIC, 
         * ADD MEMBER FUNCTION ST_IsValid RETURN INTEGER DETERMINISTIC,   
         * ADD CONSTRUCTOR FUNCTION SDO_GEOMETRY(wkt IN CLOB,             
         *          srid IN INTEGER DEFAULT NULL) RETURN SELF AS RESULT, 
         * ADD CONSTRUCTOR FUNCTION SDO_GEOMETRY(wkt IN VARCHAR2,         
         *          srid IN INTEGER DEFAULT NULL) RETURN SELF AS RESULT, 
         * ADD CONSTRUCTOR FUNCTION SDO_GEOMETRY(wkb IN BLOB,             
         *          srid IN INTEGER DEFAULT NULL) RETURN SELF AS RESULT  
         * CASCADE                                                        
         */

        #endregion

        #region Nested Types

        //Oracle Documentation for SDO_ETYPE - SIMPLE
        //Point//Line//Polygon//exterior counterclockwise - polygon ring = 1003//interior clockwise  polygon ring = 2003
        //public enum ETYPE_SIMPLE { Point = 1, LINE = 2, POLYGON = 3, POLYGON_EXTERIOR = 1003, POLYGON_INTERIOR = 2003 }

        //Oracle Documentation for SDO_ETYPE - COMPOUND
        //1005: exterior polygon ring (must be specified in counterclockwise order)
        //2005: interior polygon ring (must be specified in clockwise order)
        //public enum ETYPE_COMPOUND { FOURDIGIT = 4, POLYGON_EXTERIOR = 1005, POLYGON_INTERIOR = 2005 }

        //Oracle Documentation for SDO_GTYPE.
        //This represents the last two digits in a GTYPE, where the first item is dimension(ality) and the second is LRS

        /// <summary>
        /// Enumeration for the geometry type (SDO_GTYPE). 
        /// This reflects the document "Oracle Spatial 11gR1 Developers Guide".
        /// </summary>
        public enum Gtype 
        {
            /// <summary>
            /// Spatial ignores this geometry.
            /// </summary>
            UnknownGeometry = 0,

            /// <summary>
            /// Geometry contains one point.
            /// </summary>
            Point = 1,

            /// <summary>
            /// Geometry contains one line string that can contain straight or circular arc segments, or both. (LINE and CURVE are synonymous in this context.)
            /// </summary>
            LineCurve = 2,
            //CURVE = 2,

            /// <summary>
            /// Geometry contains one polygon with or without holes or one surface consisting of one or more 
            /// polygons. In a three-dimensional polygon, all points must be on the same plane.
            /// For a polygon with holes, enter the exterior boundary first, followed by any interior boundaries.
            /// </summary>
            PolygonSurface = 3,
            //SURFACE = 3,

            /// <summary>
            /// Geometry is a heterogeneous collection of elements. 
            /// COLLECTION is a superset that includes all other types.
            /// </summary>
            Collection = 4,

            /// <summary>
            /// Geometry has one or more points. (MULTIPOINT is a superset of Point.)
            /// </summary>
            MultiPoint = 5, // point cloud

            /// <summary>
            /// Geometry has one or more line strings. (MULTILINE and MULTICURVE are synonymous in this context,
            /// and each is a superset of both LINE and CURVE.)
            /// </summary>
            MultiLineCurve = 6, 
            //MULTICURVE = 6, 

            /// <summary>
            /// Geometry can have multiple, disjoint polygons (more than one exterior boundary), 
            /// or surfaces (MULTIPOLYGON is a superset of POLYGON, and MULTISURFACE is a superset of SURFACE.)
            /// </summary>
            MultiPolygonSurface = 7,
            //MULTISURFACE=7

            /// <summary>
            /// Geometry consists of multiple surfaces and is completely enclosed in a three-dimensional space. 
            /// Can be a cuboid or a frustum.
            /// </summary>
            Solid = 8,

            /// <summary>
            /// Geometry can have multiple, disjoint solids (more than one exterior boundary). 
            /// (MULTISOLID is a superset of SOLID.)
            /// </summary>
            MultiSolid = 9,
        }

        #endregion

        #region Fields & Constants

        /// <summary>
        /// The SRID corresponding to EPSG:4326 (WGS84),
        /// see http://de.wikipedia.org/wiki/World_Geodetic_System
        /// </summary>
        public const int SridEpsg432 = 8307; //8192;

        private int _gType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of coordinate dimensions (2, 3 or 4).
        /// </summary>
        /// <value>
        /// The coordinate dimensionality.
        /// </value>
        public int Dim { get; set; }

        /// <summary>
        /// Gets or sets the number of dimensions for the linear referencing system (LRS), 
        /// i.e. additional measure information.
        /// </summary>
        /// <value>
        /// The dimensionality for the linear referencing system (LRS).
        /// </value>
        /// <remarks>
        /// LRSDim identifies the linear referencing measure dimension for a three-dimensional linear 
        /// referencing system (LRS) geometry, that is, which dimension (3 or 4) contains the measure 
        /// value. For a non-LRS geometry, or to accept the Spatial default of the last dimension as 
        /// the measure for an LRS geometry, specify 0. For information about the linear referencing 
        /// system (LRS), see Chapter 7.
        /// </remarks>
        public int LrsDim { get; set; }

        /// <summary>
        /// Gets or sets the internal geometry type which encodes the kind of geometry (e.g. point, line, polygon, etc.),
        /// as well as the dimensionality of coordinates and linear referencing system (LRS).
        /// </summary>
        /// <value>
        /// The internal geometry type.
        /// </value>
        /// <remarks>
        /// For example, an SDO_GTYPE value of 2003 indicates a two-dimensional polygon. The number of dimensions 
        /// reflects the number of ordinates used to represent each vertex (for example, X,Y for two-dimensional objects).
        /// In any given layer (column), all geometries must have the same number of dimensions. For example, you 
        /// cannot mix two-dimensional and three-dimensional data in the same layer.
        /// </remarks>
        [OracleObjectMapping(0)]
        public decimal? SdoGType { get; set; }

        /// <summary>
        /// Gets or sets the geometry type as enumeration <see cref="Gtype"/>.
        /// </summary>
        /// <value>
        /// The geometry type as enumeration <see cref="Gtype"/>.
        /// </value>
        /// <remarks>
        /// This is a more safe way to access the geometry type.
        /// </remarks>
        public Gtype GTypeAsEnum
        {
            get { return Enum.IsDefined(typeof(Gtype), _gType) ? (Gtype)_gType : Gtype.UnknownGeometry; }
            set { _gType = (int)value; }
        }

        /// <summary>
        /// Gets or sets the spatial reference system identifier (SRID), cf. http://en.wikipedia.org/wiki/SRID
        /// However, note that although standardized by OGC, the actual values refer to
        /// Oracle's implementation. 
        /// For instance, the most accepted value for the World Geodetic System 1984 
        /// (WGS84) is 4326 (equal to the ESPG code).
        /// </summary>
        /// <value>
        /// The SRID.
        /// </value>
        /// <remarks>
        /// The SDO_SRID attribute can be used to identify a coordinate system (spatial reference system) 
        /// to be associated with the geometry. If SDO_SRID is null, no coordinate system is associated 
        /// with the geometry. If SDO_SRID is not null, it must contain a value from the SRID column of 
        /// the SDO_COORD_REF_SYS table (described in Section 6.7.9), and this value must be inserted 
        /// into the SRID column of the USER_SDO_GEOM_METADATA view (described in Section 2.8).
        /// All geometries in a geometry column must have the same SDO_SRID value if a spatial
        /// index will be built on that column.
        /// For information about coordinate systems, see Chapter 6.
        /// </remarks>
        [OracleObjectMapping(1)]
        public decimal? Srid { get; set; }

        /// <summary>
        /// Gets the SRID as integer.
        /// </summary>
        public int SridAsInt { get { return Convert.ToInt32(Srid); } }

        /// <summary>
        /// Gets or sets the point attribute.
        /// </summary>
        /// <value>
        /// The point attribute.
        /// </value>
        /// <remarks>
        /// The SDO_POINT attribute is defined using the SDO_POINT_TYPE object type, which
        /// has the attributes X, Y, and Z, all of type NUMBER. (The SDO_POINT_TYPE definition is 
        /// shown in Section 2.2.) If the SDO_ELEM_INFO and SDO_ORDINATES arrays are both null, 
        /// and the SDO_POINT attribute is non-null, then the X, Y, and Z values are considered to 
        /// be the coordinates for a point geometry. Otherwise, the SDO_POINT attribute is ignored
        /// by Spatial. You should store point geometries in the SDO_POINT attribute for optimal 
        /// storage; and if you have only point geometries in a layer, it is strongly recommended 
        /// that you store the point geometries in the SDO_POINT attribute.
        /// Section 2.7.5 illustrates a point geometry and provides examples of inserting and
        /// querying point geometries.
        /// Note: Do not use the SDO_POINT attribute in defining a linear referencing system (LRS) 
        /// point or an oriented point. For information about LRS, see Chapter 7. For information about
        /// oriented points, see Section 2.7.6.
        /// </remarks>
        [OracleObjectMapping(2)]
        public SdoPoint Point { get; set; }

        /// <summary>
        /// Gets or sets the element info array.
        /// </summary>
        /// <value>
        /// The element info array.
        /// </value>
        /// <remarks>
        /// The SDO_ELEM_INFO attribute is defined using a varying length array of numbers.
        /// This attribute lets you know how to interpret the ordinates stored in the 
        /// SDO_ORDINATES attribute (described in Section 2.2.5).
        /// Each triplet set of numbers is interpreted as follows:
        /// 
        /// <list type="bullet">
        /// <item>
        /// SDO_STARTING_OFFSET -- Indicates the offset within the SDO_ORDINATES
        /// array where the first ordinate for this element is stored. Offset values start at 1 and
        /// not at 0. Thus, the first ordinate for the first element will be at 
        /// SDO_GEOMETRY.SDO_ORDINATES(1). If there is a second element, its first ordinate will 
        /// be at SDO_GEOMETRY.SDO_ORDINATES(n), where n reflects the position within the 
        /// SDO_ORDINATE_ARRAY definition (for example, 19 for the 19th number, as in Figure 2–4 in Section 2.7.2).
        /// </item>
        /// <item>
        /// SDO_ETYPE -- Indicates the type of the element. Valid values are shown in Table 2–2.
        /// 
        /// SDO_ETYPE values 1, 2, 1003, and 2003 are considered simple elements. They are defined by 
        /// a single triplet entry in the SDO_ELEM_INFO array. For SDO_ETYPE values 1003 and 2003, the 
        /// first digit indicates exterior (1) or interior (2):
        /// 1003: exterior polygon ring (must be specified in counterclockwise order)
        /// 2003: interior polygon ring (must be specified in clockwise order)
        /// 
        /// Note: The use of 3 as an SDO_ETYPE value for polygon ring elements in a single geometry is 
        /// discouraged. You should specify 3 only if you do not know if the simple polygon is exterior 
        /// or interior, and you should then upgrade the table or layer to the current format using the 
        /// SDO_MIGRATE.TO_CURRENT procedure, described in Chapter 26.
        /// You cannot mix 1-digit and 4-digit SDO_ETYPE values in a single geometry.
        /// 
        /// SDO_ETYPE values 4, 1005, 2005, 1006, and 2006 are considered compound elements. They contain 
        /// at least one header triplet with a series of triplet values that belong to the compound element. 
        /// For 4-digit SDO_ETYPE values, the first digit indicates exterior (1) or interior (2):
        /// 1005: exterior polygon ring (must be specified in counterclockwise order)
        /// 2005: interior polygon ring (must be specified in clockwise order)
        /// 1006: exterior surface consisting of one or more polygon rings
        /// 2006: interior surface in a solid element
        /// 1007: solid element
        /// The elements of a compound element are contiguous. The last point of a subelement in a compound 
        /// element is the first point of the next subelement. The point is not repeated.
        /// </item>
        /// <item>
        /// SDO_INTERPRETATION -- Means one of two things, depending on whether or not SDO_ETYPE is a 
        /// compound element. 
        /// If SDO_ETYPE is a compound element (4, 1005, or 2005), this field specifies how
        /// many subsequent triplet values are part of the element.
        /// If the SDO_ETYPE is not a compound element (1, 2, 1003, or 2003), the interpretation attribute 
        /// determines how the sequence of ordinates for this element is interpreted. For example, a line 
        /// string or polygon boundary may be made up of a sequence of connected straight line segments 
        /// or circular arcs.
        /// Descriptions of valid SDO_ETYPE and SDO_INTERPRETATION value pairs are given in Table 2–2.
        /// </item>
        /// </list>
        /// 
        /// If a geometry consists of more than one element, then the last ordinate for an element
        /// is always one less than the starting offset for the next element. The last element in the
        /// geometry is described by the ordinates from its starting offset to the end of the SDO_ORDINATES 
        /// varying length array.
        /// 
        /// For compound elements (SDO_ETYPE values 4, 1005, or 2005), a set of n triplets (one for each 
        /// subelement) is used to describe the element. It is important to remember that subelements of 
        /// a compound element are contiguous. The last point of a subelement is the first point of the 
        /// next subelement. For subelements 1 through n-1, the end point of one subelement is the same 
        /// as the starting point of the next subelement. The starting point for subelements 2...n-2 is 
        /// the same as the end point of subelement 1...n-1. The last ordinate of subelement n is either 
        /// the starting offset minus 1 of the next element in the geometry, or the last ordinate in the 
        /// SDO_ORDINATES varying length array. 
        /// 
        /// The current size of a varying length array can be determined by using the function
        /// varray_variable.Count in PL/SQL or OCICollSize in the Oracle Call Interface (OCI).
        /// The semantics of each SDO_ETYPE element and the relationship between the SDO_ELEM_INFO 
        /// and SDO_ORDINATES varying length arrays for each of these SDO_ETYPE elements are given in Table 2–2. 
        /// </remarks>
        [OracleObjectMapping(3)]
        public decimal[] ElemInfo { get; set; }

        /// <summary>
        /// Gets or sets the ordinates array.
        /// </summary>
        /// <value>
        /// The ordinates.
        /// </value>
        /// <remarks>
        /// The SDO_ORDINATES attribute is defined using a varying length array (1048576) of
        /// NUMBER type that stores the coordinate values that make up the boundary of a
        /// spatial object. This array must always be used in conjunction with the SDO_ELEM_
        /// INFO varying length array. The values in the array are ordered by dimension. For
        /// example, a polygon whose boundary has four two-dimensional points is stored as {X1,
        /// Y1, X2, Y2, X3, Y3, X4, Y4, X1, Y1}. If the points are three-dimensional, then they are
        /// stored as {X1, Y1, Z1, X2, Y2, Z2, X3, Y3, Z3, X4, Y4, Z4, X1, Y1, Z1}. The number of
        /// dimensions associated with each point is stored as metadata in the xxx_SDO_GEOM_METADATA 
        /// views, described in Section 2.8.
        /// 
        /// The values in the SDO_ORDINATES array must all be valid and non-null. There are
        /// no special values used to delimit elements in a multielement geometry. The start and
        /// end points for the sequence describing a specific element are determined by the
        /// STARTING_OFFSET values for that element and the next element in the SDO_ELEM_INFO 
        /// array, as explained in Section 2.2.4. The offset values start at 1. SDO_ORDINATES(1) 
        /// is the first ordinate of the first point of the first element.
        /// </remarks>
        [OracleObjectMapping(4)]
        public decimal[] Ordinates { get; set; }

        /// <summary />
        [OracleCustomTypeMapping("MDSYS.SDO_ELEM_INFO_ARRAY")]
        public class ElemArrayFactory : OracleArrayTypeFactoryBase<decimal> { } // NOTE: int should suffice here!

        /// <summary />
        [OracleCustomTypeMapping("MDSYS.SDO_ORDINATE_ARRAY")]
        public class OrdinatesArrayFactory : OracleArrayTypeFactoryBase<decimal> { }

        /// <summary>
        /// Gets or sets the element info attribute as an integer array.
        /// </summary>
        /// <value>
        /// The element info attribute as an integer array.
        /// </value>
        public int[] ElemArrayOfInts
        {
            get
            {
                int[] elems = null;
                if (ElemInfo != null)
                {
                    elems = new int[ElemInfo.Length];
                    for (int k = 0; k < ElemInfo.Length; k++)
                        elems[k] = Convert.ToInt32(ElemInfo[k]);
                }
                return elems;
            }
            set
            {
                if (value != null)
                {
                    var c = value.GetLength(0);
                    ElemInfo = new decimal[c];
                    for (int k = 0; k < c; k++)
                        ElemInfo[k] = Convert.ToDecimal(value[k]);
                }
                else
                    ElemInfo = null;
            }
        }

        /// <summary>
        /// Gets or sets the ordinates attribute as an array of doubles.
        /// </summary>
        /// <value>
        /// The ordinates attribute as an array of doubles.
        /// </value>
        public double[] OrdinatesArrayOfDoubles
        {
            get
            {
                double[] elems = null;
                if (Ordinates != null)
                {
                    elems = new double[Ordinates.Length];
                    for (int k = 0; k < Ordinates.Length; k++)
                        elems[k] = Convert.ToDouble(Ordinates[k]);
                }
                return elems;
            }
            set
            {
                if (value != null)
                {
                    var c = value.GetLength(0);
                    Ordinates = new decimal[c];
                    for (int k = 0; k < c; k++)
                        Ordinates[k] = Convert.ToDecimal(value[k]);
                }
                else
                    Ordinates = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Maps from custom object.
        /// </summary>
        public override void MapFromCustomObject()
        {
            PropertiesToGType();

            SetValue(0, SdoGType);
            SetValue(1, Srid);
            SetValue(2, Point);
            SetValue(3, ElemInfo);
            SetValue(4, Ordinates);
        }

        /// <summary>
        /// Maps to custom object.
        /// </summary>
        public override void MapToCustomObject()
        {
            SdoGType = GetValue<decimal?>(0);
            Srid = GetValue<decimal?>(1);
            Point = GetValue<SdoPoint>(2);
            ElemInfo = GetValue<decimal[]>(3);
            Ordinates = GetValue<decimal[]>(4);

            PropertiesFromGType();
        }

        void PropertiesFromGType()
        {
            if (SdoGType == null) return;
            var v = (int)SdoGType;
            if (v == 0)
                return;

            Dim = v / 1000;
            v -= Dim * 1000;
            LrsDim = v / 100;
            v -= LrsDim * 100;
            _gType = v;
        }

        void PropertiesToGType()
        {
            var v = Dim * 1000;
            v = v + (LrsDim * 100);
            v = v + _gType;
            SdoGType = Convert.ToDecimal(v);
        }

        #endregion
    }
}
