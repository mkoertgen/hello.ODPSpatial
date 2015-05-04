using Oracle.DataAccess.Types;

namespace ODPSpatial
{
    /// <summary>
    /// This is a user defined type to map the SQL/MM (3) ST_GEOMETRY
    /// to a .NET class
    /// </summary>
    [OracleCustomTypeMapping("MDSYS.ST_GEOMETRY")]
    public class StGeometry : OracleCustomTypeBase<StGeometry>
    {
        #region Documentation

        /* To verifiy the mapping, execute the SQL-statement "describe MDSYS.ST_GEOMETRY":
         * 
         * user type definition
         * -------------------------------------------------------------------------------- 
         * TYPE ST_GEOMETRY                                                                 
         * AS OBJECT (                             
         *      GEOM SDO_GEOMETRY,                                                             
         *      MEMBER FUNCTION GET_SDO_GEOM RETURN SDO_GEOMETRY DETERMINISTIC,                
         *      MEMBER FUNCTION GET_WKB RETURN BLOB DETERMINISTIC,                             
         *      MEMBER FUNCTION GET_WKT RETURN CLOB DETERMINISTIC,                             
         *      MEMBER FUNCTION ST_CoordDim RETURN SMALLINT DETERMINISTIC,                     
         *      MEMBER FUNCTION ST_IsValid RETURN INTEGER DETERMINISTIC,                       
         *      MEMBER FUNCTION ST_SRID RETURN INTEGER,                                        
         *      MEMBER FUNCTION ST_SRID(asrid INTEGER) RETURN ST_Geometry,                     
         *      STATIC FUNCTION FROM_WKT(wkt CLOB) RETURN ST_GEOMETRY DETERMINISTIC,           
         *      STATIC FUNCTION FROM_WKT(wkt VARCHAR2) RETURN ST_GEOMETRY DETERMINISTIC,       
         *      STATIC FUNCTION FROM_WKB(wkb BLOB) RETURN ST_GEOMETRY DETERMINISTIC,           
         *      STATIC FUNCTION FROM_WKT(wkt CLOB, asrid INTEGER)                              
         *      RETURN ST_GEOMETRY DETERMINISTIC,                                    
         *      STATIC FUNCTION FROM_WKT(wkt VARCHAR2, asrid INTEGER)                          
         *      RETURN ST_GEOMETRY DETERMINISTIC,                                    
         *      STATIC FUNCTION FROM_WKB(wkb BLOB, asrid INTEGER)                              
         *          RETURN ST_GEOMETRY DETERMINISTIC,                                    
         *      STATIC FUNCTION FROM_SDO_GEOM(ageometry SDO_GEOMETRY)                          
         *          RETURN ST_GEOMETRY DETERMINISTIC) NOT FINAL                          
         * 
         * ALTER TYPE ST_GEOMETRY 
         *      ADD MEMBER FUNCTION ST_IsEmpty RETURN Integer DETERMINISTIC,                       
         *      ADD MEMBER FUNCTION ST_Envelope RETURN ST_Geometry DETERMINISTIC,              
         *      ADD MEMBER FUNCTION ST_Boundary RETURN ST_GEOMETRY DETERMINISTIC,              
         *      ADD MEMBER FUNCTION ST_GeometryType RETURN VARCHAR2 DETERMINISTIC,             
         *      ADD MEMBER FUNCTION ST_Buffer(d NUMBER) RETURN ST_Geometry DETERMINISTIC,      
         *      ADD MEMBER FUNCTION ST_Equals(g2 ST_Geometry) RETURN Integer DETERMINISTIC,    
         *      ADD MEMBER FUNCTION ST_SymmetricDifference(g2 ST_Geometry)                     
         *          RETURN ST_Geometry DETERMINISTIC,                                  
         *      ADD MEMBER FUNCTION ST_Distance(g2 ST_Geometry) RETURN NUMBER DETERMINISTIC,   
         *      ADD MEMBER FUNCTION ST_IsSimple RETURN Integer DETERMINISTIC,                  
         *      ADD MEMBER FUNCTION ST_Intersects(g2 ST_Geometry)                              
         *          RETURN Integer DETERMINISTIC,                                            
         *      ADD MEMBER FUNCTION ST_Relate(g2 ST_Geometry, PatternMatrix VARCHAR2)          
         *          RETURN Integer DETERMINISTIC,                                      
         *      ADD MEMBER FUNCTION ST_Dimension RETURN Integer DETERMINISTIC,                 
         *      ADD MEMBER FUNCTION ST_Cross(g2 ST_Geometry) RETURN Integer DETERMINISTIC,     
         *      ADD MEMBER FUNCTION ST_Disjoint(g2 ST_Geometry) RETURN Integer DETERMINISTIC,  
         *      ADD MEMBER FUNCTION ST_Touch(g2 ST_Geometry) RETURN Integer DETERMINISTIC,     
         *      ADD MEMBER FUNCTION ST_Within(g2 ST_Geometry) RETURN Integer DETERMINISTIC,    
         *      ADD MEMBER FUNCTION ST_Overlap(g2 ST_Geometry) RETURN Integer DETERMINISTIC,   
         *      ADD MEMBER FUNCTION ST_Contains(g2 ST_Geometry) RETURN Integer DETERMINISTIC,  
         *      ADD MEMBER FUNCTION ST_Intersection(g2 ST_Geometry)                            
         *          RETURN ST_Geometry DETERMINISTIC,                                    
         *      ADD MEMBER FUNCTION ST_Difference(g2 ST_Geometry)                              
         *          RETURN ST_Geometry DETERMINISTIC,                                   
         *      ADD MEMBER FUNCTION ST_Union(g2 ST_Geometry) RETURN ST_Geometry DETERMINISTIC, 
         *      ADD MEMBER FUNCTION ST_ConvexHull RETURN ST_Geometry DETERMINISTIC,            
         *      ADD MEMBER FUNCTION ST_Centroid RETURN ST_Geometry DETERMINISTIC               
         *      CASCADE                                                                      
         * 
         * ALTER TYPE ST_GEOMETRY                                                         
         *      ADD MEMBER FUNCTION ST_SymDifference(g2 ST_Geometry)                             
         *          RETURN ST_Geometry DETERMINISTIC,                                  
         *      ADD MEMBER FUNCTION ST_Touches(g2 ST_Geometry) RETURN Integer DETERMINISTIC,   
         *      ADD MEMBER FUNCTION ST_Crosses(g2 ST_Geometry) RETURN Integer DETERMINISTIC,   
         *      ADD MEMBER FUNCTION ST_GetTolerance RETURN NUMBER DETERMINISTIC                
         *      CASCADE                                                                       
         */

        #endregion

        /// <summary>
        /// Gets or sets the geometry value.
        /// </summary>
        /// <value>
        /// The geometry value.
        /// </value>
        [OracleObjectMapping(0)]
        public SdoGeometry Geom { get; set; }

        /// <summary>
        /// Maps from custom object.
        /// </summary>
        public override void MapFromCustomObject()
        {
            SetValue(0, Geom); //"GEOM", geom);
        }

        /// <summary>
        /// Maps to custom object.
        /// </summary>
        public override void MapToCustomObject()
        {
            Geom = GetValue<SdoGeometry>(0); //"GEOM");
        }
    }
}
