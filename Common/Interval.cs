using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FutureConcepts.Media
{
    /// <summary>
    /// Used to represent Intervals for Forward Error Correction and Guard Intervals
    /// </summary>
    [Serializable]
    [DataContract]
    public class Interval
    {
        #region Pre Defined Intervals

        /// <summary>
        /// Auto-detect
        /// </summary>
        public static readonly Interval Auto = null;
        /// <summary>
        /// 1/32
        /// </summary>
        public static readonly Interval _1_32 = new Interval(1, 32);
        /// <summary>
        /// 1/16
        /// </summary>
        public static readonly Interval _1_16 = new Interval(1, 16);
        /// <summary>
        /// 1/8
        /// </summary>
        public static readonly Interval _1_8 = new Interval(1, 8);
        /// <summary>
        /// 1/4
        /// </summary>
        public static readonly Interval _1_4 = new Interval(1, 4);
        /// <summary>
        /// 1/3
        /// </summary>
        public static readonly Interval _1_3 = new Interval(1, 3);
        /// <summary>
        /// 1/2
        /// </summary>
        public static readonly Interval _1_2 = new Interval(1, 2);
        /// <summary>
        /// 2/3
        /// </summary>
        public static readonly Interval _2_3 = new Interval(2, 3);
        /// <summary>
        /// 3/4
        /// </summary>
        public static readonly Interval _3_4 = new Interval(3, 4);
        /// <summary>
        /// 5/6
        /// </summary>
        public static readonly Interval _5_6 = new Interval(5, 6);
        /// <summary>
        /// 7/8
        /// </summary>
        public static readonly Interval _7_8 = new Interval(7, 8);

        #endregion

        /// <summary>
        /// Creates 0/0
        /// </summary>
        public Interval() { }

        /// <summary>
        /// Creates the specified Interval
        /// </summary>
        /// <param name="numerator">numerator</param>
        /// <param name="denominator">denominator</param>
        public Interval(int numerator, int denominator)
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }

        /// <summary>
        /// Clones the specified interval
        /// </summary>
        /// <param name="clone">interval to clone</param>
        public Interval(Interval clone) : this(clone.Numerator, clone.Denominator) { }

        private int _num;
        /// <summary>
        /// The upper number
        /// </summary>
        [DataMember]
        public int Numerator
        {
            get
            {
                return _num;
            }
            set
            {
                _num = value;
            }
        }

        private int _den;
        /// <summary>
        /// The lower number
        /// </summary>
        [DataMember]
        public int Denominator
        {
            get
            {
                return _den;
            }
            set
            {
                _den = value;
            }
        }

        /// <summary>
        /// Shows the interval
        /// </summary>
        /// <returns>the numerator/denominator</returns>
        public override string ToString()
        {
            return Numerator + "/" + Denominator;
        }

        /// <summary>
        /// determines if two intervals are literally the same (doesn't do math)
        /// </summary>
        /// <param name="obj">other object to compare to</param>
        /// <returns>true if equal, false if not</returns>
        public override bool Equals(object obj)
        {
            Interval rhs = obj as Interval;

            if (rhs == null)
            {
                return base.Equals(obj);
            }
            else
            {
                return (rhs.Numerator == this.Numerator) && (rhs.Denominator == this.Denominator);
            }
        }

        /// <summary>
        /// Returns the hash code
        /// </summary>
        /// <returns>the hash code for this interval</returns>
        public override int GetHashCode()
        {
            return this.Numerator.GetHashCode() ^ this.Denominator.GetHashCode();
        }
    }
}
