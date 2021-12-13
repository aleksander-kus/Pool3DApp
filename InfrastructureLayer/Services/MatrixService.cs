using System;

namespace InfrastructureLayer.Services
{
    public class MatrixService
    {
        public double[] Multiply(double[,] matrix1, double[] vector)
        {
            // cahing matrix lengths for better performance  
            var matrix1Rows = 4;
            var matrix1Cols = 4;
            var matrix2Rows = 4;

            // checking if product is defined  
            if (matrix1Cols != matrix2Rows)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            // creating the final product matrix  
            double[] product = new double[4];

            // looping through matrix 1 rows  
            for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++)
            {
                // loop through matrix 1 columns to calculate the dot product  
                for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++)
                {
                    product[matrix1_row] +=
                        matrix1[matrix1_row, matrix1_col] *
                        vector[matrix1_col];
                }
            }

            return product;
        }
    }
}
