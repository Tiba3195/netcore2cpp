using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;

namespace DotNetLib
{
    public static class MathHelpers
    {
        public const float SMALL_NUMBER = (0.000001f);
        public const float PI = (3.1415926535897932f);  /* Extra digits if needed: 3.1415926535897932384626433832795f */
        public const float KINDA_SMALL_NUMBER = (1.0e-4f);
        public const float BIG_NUMBER = (3.4e+38f);
        public const float EULERS_NUMBER = (2.71828182845904523536f);
        public const float UE_GOLDEN_RATIO = (1.6180339887498948482045868343656381f);    /* Also known as divine proportion, golden mean, or golden section - related to the Fibonacci Sequence = (1 + sqrt(5)) / 2 */     

        // Copied from float.h
        public const float MAX_FLT = 3.402823466e+38F; 
        // Aux constants.
        public const float INV_PI = (0.31830988618f);
        public const float HALF_PI = (1.57079632679f);
        // Common square roots
        public const double UE_SQRT_2 = (1.4142135623730950488016887242097f);
        public const double UE_SQRT_3 = (1.7320508075688772935274463415059f);
        public const double UE_INV_SQRT_2 = (0.70710678118654752440084436210485f);
        public const double UE_INV_SQRT_3 = (0.57735026918962576450914878050196f);
        public const double UE_HALF_SQRT_2 = (0.70710678118654752440084436210485f);
        public const double UE_HALF_SQRT_3 = (0.86602540378443864676372317075294f);

        // Magic numbers for numerical precision.
        public const float DELTA = (0.00001f);


        /** For the given Value clamped to the [Input:Range] inclusive, returns the corresponding percentage in [Output:Range] Inclusive. */
        public static float GetMappedRangeValueClamped(PointF InputRange, PointF OutputRange, float Value)
        {

            float ClampedPct = Clamp(GetRangePct(InputRange, Value), 0.0f, 1.0f);
            return GetRangeValue(OutputRange, ClampedPct);
        }

        /** Transform the given Value relative to the input range to the Output Range. */
        public static float GetMappedRangeValueUnclamped(PointF InputRange, PointF OutputRange, float Value)
        {
            return GetRangeValue(OutputRange, GetRangePct(InputRange, Value));
        }

        public static float GetRangeValue(PointF Range, float Pct)
        {
            return Lerp(Range.X, Range.Y, Pct);
        }

        public static float Lerp(float A, float B, float Alpha)
        {
            return (A + Alpha * (B - A));
        }

        public static float GetRangePct(PointF Range, float Value)
        {
            return GetRangePct(Range.X, Range.Y, Value);
        }

        /** Calculates the percentage along a line from MinValue to MaxValue that Value is. */
        public static float GetRangePct(float MinValue, float MaxValue, float Value)
        {
            // Avoid Divide by Zero.
            // But also if our range is a point, output whether Value is before or after.
            float Divisor = MaxValue - MinValue;
            if (IsNearlyZero(Divisor))
            {
                return (Value >= MaxValue) ? 1.0f : 0.0f;
            }

            return (Value - MinValue) / Divisor;
        }

        public static bool IsNearlyZero(float Value, float ErrorTolerance = SMALL_NUMBER)
        {
            return Abs(Value) <= ErrorTolerance;
        }

        public static float Clamp(float X, float Min, float Max)
        {
            return X < Min ? Min : X < Max ? X : Max;
        }

        /** Computes absolute value in a generic way */
        public static float Abs(float A)
        {
            return (A >= 0) ? A : -A;
        }

        /** Returns 1, 0, or -1 depending on relation of T to 0 */
        public static float Sign(float A)
        {
            return (A > 0) ? 1 : ((A < 0) ? -1 : 0);
        }

        /** Returns higher value in a generic way */
        public static float Max(float A, float B)
        {
            return (A >= B) ? A : B;
        }

        /** Returns lower value in a generic way */
        public static float Min(float A, float B)
        {
            return (A <= B) ? A : B;
        }
        /** Multiples value by itself */
        public static float Square(float A)
        {
            return A * A;
        }


        /** Return true if value is NaN (not a number). */
        public static bool IsNaN(float A)
        {
            return (((UInt32)A) & 0x7FFFFFFF) > 0x7F800000;
        }
        /** Return true if value is finite (not NaN and not Infinity). */
        public static bool IsFinite(float A)
        {
            return (((UInt32)A) & 0x7F800000) != 0x7F800000;
        }
        public static bool IsNegativeFloat(float A)
        {
            return (((UInt32)A) >= (UInt32)0x80000000); // Detects sign bit.
        }

        public static bool IsNegativeDouble(double A)
        {
            return (((UInt64)A) >= (UInt64)0x8000000000000000); // Detects sign bit.
        }

        /**
        * Returns value based on comparand. The main purpose of this function is to avoid
        * branching based on floating point comparison which can be avoided via compiler
        * intrinsics.
        *
        * Please note that we don't define what happens in the case of NaNs as there might
        * be platform specific differences.
        *
        * @param	Comparand		Comparand the results are based on
        * @param	ValueGEZero		Return value if Comparand >= 0
        * @param	ValueLTZero		Return value if Comparand < 0
        *
        * @return	ValueGEZero if Comparand >= 0, ValueLTZero otherwise
        */
        public static float FloatSelect(float Comparand, float ValueGEZero, float ValueLTZero)
        {
            return Comparand >= 0.0f ? ValueGEZero : ValueLTZero;
        }

        /**
         * Returns value based on comparand. The main purpose of this function is to avoid
         * branching based on floating point comparison which can be avoided via compiler
         * intrinsics.
         *
         * Please note that we don't define what happens in the case of NaNs as there might
         * be platform specific differences.
         *
         * @param	Comparand		Comparand the results are based on
         * @param	ValueGEZero		Return value if Comparand >= 0
         * @param	ValueLTZero		Return value if Comparand < 0
         *
         * @return	ValueGEZero if Comparand >= 0, ValueLTZero otherwise
         */
        public static double FloatSelect(double Comparand, double ValueGEZero, double ValueLTZero)
        {
            return Comparand >= 0.0f ? ValueGEZero : ValueLTZero;
        }

        /** Clamps X to be between Min and Max, wrapped */
        public static float ClampWrap(float X, float Min, float Max)
        {
            return X < Min ? Max : X <= Max ? X : Min;
        }


        /** 
        * Converts radians to degrees.
         * @param	RadVal			Value in radians.
        * @return					Value in degrees.
        */

        public static float RadiansToDegrees(float RadVal)
        {
            return RadVal * (180.0f / PI);
        }

        /** 
         * Converts degrees to radians.
         * @param	DegVal			Value in degrees.
         * @return					Value in radians.
         */

        public static float DegreesToRadians(float DegVal)
        {
            return DegVal * (PI / 180.0f);
        }

        public static float FInterpTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            // If no interp speed, jump to target value
            if (InterpSpeed <= 0.0f)
            {
                return Target;
            }

            // Distance to reach
            float Dist = Target - Current;

            // If distance is too small, just set the desired location
            if (Square(Dist) < SMALL_NUMBER)
            {
                return Target;
            }

            // Delta Move, Clamp so we do not over shoot.
            float DeltaMove = Dist * Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);

            return Current + DeltaMove;
        }

        public static float FInterpConstantTo(float Current, float Target, float DeltaTime, float InterpSpeed)
        {
            float Dist = Target - Current;

            // If distance is too small, just set the desired location
            if (Square(Dist) < SMALL_NUMBER)
            {
                return Target;
            }

             float Step = InterpSpeed * DeltaTime;
            return Current + Clamp(Dist, -Step, Step);
        }


        public struct CANHeader
        {
            public Int32 priorityID;
            public Int32 arbitrationID;
            public Int32 senderID;

            public override string ToString()
            {


               string arbitrationIDstr = @"0x" + arbitrationID.ToString("X2");
               string senderIDstr =@"0x" + senderID.ToString("X2");

                return @"0x" + priorityID + @", " + arbitrationIDstr + @", " + senderIDstr;
            }
        };


        public static CANHeader decode(Int32 _header)
        {
            CANHeader OutputHeader = new CANHeader();
            if (_header < 0x800)
            {
                // 11-bit header
                OutputHeader.arbitrationID = (_header >> 0) & 0x7FF;
            }
            else
            {
                // 29-bit header
                OutputHeader.priorityID = (_header >> 26) & 0x7;
                OutputHeader.arbitrationID = (_header >> 13) & 0x1FFF;
                OutputHeader.senderID = (_header >> 0) & 0x1FFF;
            }
            return OutputHeader;
        }
        public static Int32 encode29bit(CANHeader header)
        {
            Int32 buffer = 0;
            buffer = (buffer << 3) | 0x0; // 3 bit padding
            buffer = (buffer << 3) | header.priorityID;
            buffer = (buffer << 13) | header.arbitrationID;
            buffer = (buffer << 13) | header.senderID;
            return buffer;
        }

        public static Int16 encode11bit(CANHeader header)
        {
            Int16 buffer = 0;
            buffer =(Int16)((buffer << 5) | 0x0); // 5 bit padding
            buffer = (Int16)((buffer << 11) | header.arbitrationID);
            return buffer;
        }



    }

}
