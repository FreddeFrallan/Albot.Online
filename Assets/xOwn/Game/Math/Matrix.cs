using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace Game {
namespace Math {

// This really works on all numerical types, but can't get generics to work properly
public class Matrix : IEnumerable<int> {
	// Exceptions
	public class BoundsException : Exception { 
		public BoundsException(int x, int y) : base("x: " + x.ToString() + ", y: " + y.ToString()) {} 
	}
	public class IncompatibleSize : Exception {
		public IncompatibleSize() {}
		public IncompatibleSize(int[] a, int[] b) : base(a.ToString() + " and " + b.ToString()) {}
	}

	// Members
	private List<int> mRep;
	private int mxDim, myDim;

	public Matrix(int xDim, int yDim) {
		mxDim = xDim;
		myDim = yDim;
		mRep = new List<int>();
		for (int i = 0; i < mxDim*myDim; ++i) mRep.Add(0);
	}

	public Matrix(int xDim, int yDim, List<int> values) {
		if (values.Count != xDim*yDim)
			throw new IncompatibleSize();
		mxDim = xDim;
		myDim = yDim;
		mRep = values;
	}

	public void set(int x, int y, int v) { mRep[index(x, y)] = v; }
	public int at(int x, int y) { return mRep[index(x, y)]; }
	public int[] dim() { return new int[2]{mxDim, myDim}; } 

	public Matrix convolve(Matrix cmat) {
		int nx = mxDim - cmat.mxDim + 1;
		int ny = myDim - cmat.myDim + 1;
		if (nx < 1 || ny < 1) throw new IncompatibleSize(dim(), cmat.dim());
		Matrix outMat = new Matrix(nx, ny);
		for (int ox = 0; ox < nx; ++ox) {
			for (int oy = 0; oy < ny; ++oy) {
				for (int cx = 0; cx < cmat.mxDim; ++cx) {
					for (int cy = 0; cy < cmat.myDim; ++cy) {
						int v = outMat.at(ox, oy) + at(ox + cx, oy + cy)*cmat.at(cx, cy);
						outMat.set(ox, oy, v);
					}
				}
			}
		}
		return outMat;
	}

	public void print() {
		for (int y = myDim - 1; y >= 0; --y) {
			string s = "";
			for (int x = 0; x < mxDim; ++x) {
				s += at(x, y).ToString() + " ";
			}
			Debug.Log(s);
		}
	}

	private int index(int x, int y) {
		if (x >= mxDim || x < 0 || y >= myDim || y < 0)
			throw new BoundsException(x, y);
		return x*myDim + y;
	}

	public IEnumerator<int> GetEnumerator() { return mRep.GetEnumerator(); }
	IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)mRep).GetEnumerator(); } 
}

} // namespace Math
} // namespace Game
