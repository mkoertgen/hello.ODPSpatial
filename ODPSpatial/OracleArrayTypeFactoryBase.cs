using System;
using Oracle.DataAccess.Types;

namespace ODPSpatial
{
    /// <summary>
    /// Abstract base class for ODP.NET to create arrays that represent Oracle Collections.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OracleArrayTypeFactoryBase<T> : IOracleArrayTypeFactory
    {
        /// <summary>
        /// Returns a new array of the specified length to store Oracle Collection elements.
        /// </summary>
        /// <param name="numElems">The number of collection elements to be returned.</param>
        /// <returns></returns>
        public Array CreateArray(int numElems)
        {
            return new T[numElems];
        }

        /// <summary>
        /// Returns a newly allocated OracleUdtStatus array of the specified length that will 
        /// be used to store the null status of the collection elements
        /// </summary>
        /// <param name="numElems">The number of collection elements to be returned.</param>
        /// <returns></returns>
        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
