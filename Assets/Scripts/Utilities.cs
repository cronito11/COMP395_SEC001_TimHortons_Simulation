using System;
using System.Runtime.InteropServices;
using UnityEngine;
internal class Utilities
{
    internal static float GetExp(float u, float lambda)
    {
        //throw new NotImplementedException();
        return -Mathf.Log(1 - u) / lambda;
    }

    internal static float GetTriangularDistribution(float u, float a, float b, float c)
    {
        //Ref: https://en.wikipedia.org/wiki/Triangular_distribution
        //public float a = 3, b = 7, c = 5; // You should have c in (a,b)   a<c<b

        //throw new NotImplementedException();
        float V = (c - a) / (b - a);
        //print("V=" + V);
        float res = 0f;
        if (u < V)
        {
            res = a + Mathf.Sqrt(u * (b - a) * (c - a));
        }
        else if (u>= V)
        {
            res = b - Mathf.Sqrt((1-u)* (b - a) * (b- c));
        } 
        else{
            //print("Wrong U, or a or b or c; u is in [0,1), c is in (a,b)");
            Console.WriteLine("Wrong U, or a or b or c; u is in [0,1), c is in (a,b)");
        }



        return res;
    }


    //LinearRegression
    //xs[i], ys[i], i in [0,N-1]
    //y=A*x+B   , ys[i]=A*xs[i]+B + e_i
    // when r close to 1, almost perfect increasing line fit
    // when r close to -1, almost perfect decreasing line fit
    // when r close to 0, no correlation

    public static void LinearRegression(float[] xs, float[] ys, out float A, out float B, out float r_xy, out float MSE)
    {
        float sx = 0, sy = 0, sxx = 0, sxy = 0, syy = 0;
        int n = xs.Length;
        for (int i = 0; i < n; i++)
        {
            sx += xs[i];
            sy += ys[i];
            sxy += xs[i] * ys[i];
            sxx += xs[i] * xs[i];
            syy += ys[i] * ys[i];
        }

        A = (n * sxy - sx * sy) / (n * sxx - sx * sx);
        B = (sy - A * sx) / n;
        r_xy = (n * sxy - sx * sy) / Mathf.Sqrt((n * sxx - sx * sx) * (n * syy - sy * sy));
        MSE = 0;
        for (int i = 0; i < n; i++)
        {
            float e_i = (ys[i] - A * xs[i] - B);
            MSE += e_i * e_i;
        }
        MSE /= n;
    }
    //Euler Method
    public static void EulerMethod(float x_i, float y_i, float step, Func<float, float, float> f, out float new_x, out float new_y)
    {
        new_x = x_i + step;
        new_y = y_i + step * f(x_i, y_i); //Euler Scheme

    }

    //RK4
    public static void RK4(float x_i, float y_i, float step, Func<float, float, float> f, out float new_x, out float new_y)
    {
        float x1 = x_i;
        float x2 = x_i + step / 2;
        float x3 = x_i + step / 2;
        float x4 = x_i + step;

        float y1, y2, y3, y4, K1, K2, K3, K4, K;
        y1 = y_i;
        K1 = f(x1, y1);

        y2 = y_i + step / 2 * K1;
        K2 = f(x2, y2);

        y3 = y_i + step / 2 * K2;
        K3 = f(x3, y3);

        y4 = y_i + step * K3;
        K4 = f(x4, y4);

        K = (K1 + 2 * K2 + 2 * K3 + K4) / 6f;

        new_x = x_i + step;
        new_y = y_i + step * K; //RK4 Scheme

    }
    public static float MultiInverseInterpolate(float[] x, float[] cum_freqs, float u)
    {
        float xval = 0;
        if(u<= cum_freqs[0])
            return x[0];

        for (int i = 0; i < cum_freqs.Length - 1; i++)
        {
            //guard for flat segment of distribution
            if (Mathf.Abs(cum_freqs[i + 1] - cum_freqs[i]) < float.Epsilon)
                return x[i];
            //we have the segment; interpolate
            if (cum_freqs[i] <= u && u <= cum_freqs[i + 1])
            {
                xval = x[i] + (u - cum_freqs[i]) * (x[i + 1] - x[i]) / (cum_freqs[i + 1] - cum_freqs[i]);
                break;
            }
        }
        if(xval ==0 && u> cum_freqs[cum_freqs.Length -1])
            xval = x[cum_freqs.Length - 1];

        return xval;
    }

    //MultiInterpolation
    public static float MultiInterpolate(float[] xs, float[] ys, float x)
    {
        float y = 0;
        for (int i = 0; i < xs.Length - 1; i++)
        {
            //guard in case we have a flat segment in the cumulative distribution
            if (Mathf.Abs(xs[i + 1] - xs[i]) <= float.Epsilon)
            {
                //return ys[i];//
                //return ys[i+1];//
                return (ys[i] + ys[i + 1]) / 2;

            }
            if (x >= xs[i] && x < xs[i + 1])
            {
                return Mathf.Lerp(ys[i], ys[i + 1], x / (xs[i + 1] - xs[i]));
            }

        }
        return y;
    }

}