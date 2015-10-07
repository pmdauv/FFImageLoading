﻿using System;
using FFImageLoading.Work;
using Android.Graphics;
using System.Linq;

namespace FFImageLoading.Transformations
{
	public class ColorSpaceTransformation : TransformationBase
	{
		ColorMatrix _colorMatrix;

		public ColorSpaceTransformation(float[][] rgbawMatrix)
		{
			_colorMatrix = new ColorMatrix();
			UpdateColorMatrix(rgbawMatrix);
		}

		public ColorSpaceTransformation(ColorMatrix colorMatrix)
		{
			_colorMatrix = colorMatrix;
		}

		public override void SetParameters(object[] parameters)
		{
			float[][] rgbawMatrix = (float[][])parameters[0];
			UpdateColorMatrix(rgbawMatrix);
		}

		public override string Key
		{
			get { return "ColorSpaceTransformation"; }
		}

		void UpdateColorMatrix(float[][] rgbawMatrix)
		{
			var rOffset = rgbawMatrix[0][4];
			var gOffset = rgbawMatrix[1][4];
			var bOffset = rgbawMatrix[2][4];
			var aOffset = rgbawMatrix[3][4];

			_colorMatrix.SetScale(rOffset, gOffset, bOffset, aOffset);
			var transposed = GetAndroidMatrix(rgbawMatrix);			
			_colorMatrix.Set(transposed);
		}

		static float[] GetAndroidMatrix(float[][] rgbawMatrix)
		{
			var transposed = new float[20];
			int counter = 0;

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					transposed[counter] = rgbawMatrix[j][i];
					counter++;
				}
			}

			return transposed;
		}

		protected override Bitmap Transform(Bitmap source)
		{
			try
			{
				var transformed = ToColorSpace(source, _colorMatrix);
				return transformed;
			}
			finally
			{
				source.Recycle();
			}
		}

		public static Bitmap ToColorSpace(Bitmap source, ColorMatrix colorMatrix)
		{
			int width = source.Width;
			int height = source.Height;

			Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

			using (Canvas canvas = new Canvas(bitmap))
			using (Paint paint = new Paint())
			{
				paint.SetColorFilter(new ColorMatrixColorFilter(colorMatrix));
				canvas.DrawBitmap(source, 0, 0, paint);

				return bitmap;	
			}
		}
	}
}
