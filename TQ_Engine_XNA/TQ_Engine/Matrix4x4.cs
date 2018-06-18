using System.Collections;
using Microsoft.Xna.Framework;
using TQ;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Matrix2x2
{
	private float angleGetter = 0;
	public float angle
	{
		get
		{
			return angleGetter;
		}
		set
		{
			Vector basisX = GetBasis(value);
			Vector basisY = GetBasis(value + 90);
			a = basisX.x;
			b = basisX.y;
			c = basisY.x;
			d = basisY.y;
		}
	}
	public float a = 1;
	public float b = 0;
	public float c = 0;
	public float d = 1;
	public float scale = 1;
	public static Vector GetBasis (float angle) {
		float x = Mathf.Cos (angle * Mathf.Deg2Rad);
		float y = Mathf.Sin (angle * Mathf.Deg2Rad);
		return new Vector (x, y);
	}
	public static Vector operator * (Vector b, Matrix2x2 m) {
		b *= m.scale;
		Vector x = new Vector (m.a, m.b) * b.x;
		Vector y = new Vector (m.c, m.d) * b.y;
		return x + y;
	}
}
[System.Serializable]
public class Matrix4x4
{
    private float a = 0;
	private float b = 0;
	private float c = 0;
	private float d = 0;
	private float e = 0;
	private float f = 0;
	private float g = 0;
	private float h = 0;
	private float i = 0;

	public Matrix4x4 () {
        Init();
		scale = Vector.one;
	}
	public Matrix4x4 (Vector _position, Vector _euler, Vector _scale) {
        Init();
		scale = _scale;
		forward = Vector.forward;
		right = Vector.right;
		up = Vector.up;
		localEulerAngles = _euler;
		localPosition = _position;
	}

    public void Init()
    {
        forward = Vector.forward;
        right = Vector.right;
        up = Vector.up;

        childs = new List<Matrix4x4>();

        onChanged = () =>
        {
            SetBasis(localEulerAngles);
            foreach (var ch in childs)
            {
                ch.onChanged();
            }
        };
    }

	public Matrix4x4 parent { get; private set; }
    public List<Matrix4x4> childs { get; private set; }

    public delegate void OnChanged();

    public OnChanged onChanged { get; private set; }

    public void SetParent(Matrix4x4 _parent)
    {
        if (parent != null)
        {
            parent.childs.Remove(this);
        }
        if (_parent != null)
        {
            _parent.childs.Add(this);
        }
        parent = _parent;
    }

    public Matrix rotationMatrix
    {
        get
        {
            Matrix x = Matrix.CreateWorld(globalPosition, forward, up);
            return x;
        }
    }

    public Vector globalEulerAngles
    {
        get
        {
            Vector euler = localEulerAngles;
            euler = parent != null ? euler + parent.globalEulerAngles : euler;
            return euler;
        }
        set
        {
            Vector delta = value - globalEulerAngles;
            localEulerAngles += delta;
        }
    }

    public Vector globalPosition
    {
        get
        {
            Vector pos = localPosition;
            pos = parent != null ? (pos > parent) + parent.globalPosition : pos;
            return pos;
        }
        set
        {
            Vector delta = value - globalPosition;
            Vector plus = parent != null ? delta / parent : delta;
            localPosition += plus;
        }
    }

	public static Vector ClampEuler (Vector origin) {
		return new Vector(origin.x % 360, origin.y % 360, origin.z % 360);
	}

	public Vector localEulerAngles
	{
		get
		{
			return ClampEuler (eulerAnglesGetter);
		}
		set
		{
			eulerAnglesGetter = ClampEuler(value);
            onChanged();
		}
	}
	private Vector eulerAnglesGetter = Vector.zero;

    public static Matrix4x4[] GetIerarchyToRootFrom(Matrix4x4 fr)
    {
        Matrix4x4 p = fr.parent;
        List<Matrix4x4> i = new List<Matrix4x4>();
        while (p != null)
        {
            i.Add(p);
            p = p.parent;
        }
        return i.ToArray();
    }

	public Vector scale { get; set; }
    private Vector localPosition_get;
    public Vector localPosition
    {
        get
        {
            return localPosition_get;
        }
        set
        {
            localPosition_get = value;
            onChanged();
        }
    }

	public static Vector Cross (Vector a, Vector b) {
		a = a.normalized;
		b = b.normalized;
		return new Vector (a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x).normalized;
	}

	public Vector forward
	{
		get
		{
			return new Vector(a, d, g).normalized;
		}
		protected set
		{
			Vector v = value.normalized;
			a = v.x;
			d = v.y;
			g = v.z;
		}
	}
	public Vector up
	{
		get
		{
			return new Vector(b, e, h).normalized;
		}
        protected set
		{
			Vector v = value.normalized;
			b = v.x;
			e = v.y;
			h = v.z;
		}
	}

	public Vector right
	{
		get
		{
			return new Vector(c, f, i).normalized;
		}
        protected set
		{
			Vector v = value.normalized;
			c = v.x;
			f = v.y;
			i = v.z;
		}
	}

	public void SetBasis (Vector euler) {
		Matrix2x2 main = new Matrix2x2 ();
		main.angle = euler.y;
		float zUp = Mathf.Sin (euler.x * Mathf.Deg2Rad);
		float zCos = Mathf.Cos (euler.x * Mathf.Deg2Rad);
		Vector u = (Vector.up * main).normalized * zCos;
		Vector zAxis = new Vector (-u.x, -zUp, u.y).normalized;
		float xUp = Mathf.Sin (euler.z * Mathf.Deg2Rad);
		float xCos = Mathf.Cos (euler.z * Mathf.Deg2Rad);
		main.angle = euler.y + 90;
		u = (Vector.up * main).normalized * xCos;
		Vector xAxis = new Vector (-u.x, xUp, u.y).normalized;
		Vector yAxis = -Cross (xAxis, zAxis).normalized;
		xAxis = -Cross (zAxis, yAxis);
		forward = zAxis;
		up = yAxis;
		right = xAxis;

        if (parent != null)
        {
            forward = forward > parent;
            up = up > parent;
            right = right > parent;
        }
    }

	public static Vector operator * (Vector b, Matrix4x4 m) {
		return (b > m) + m.globalPosition;
	}
	public static Vector operator > (Vector b, Matrix4x4 m) {
		b = new Vector (b.x * m.scale.x, b.y * m.scale.y, b.z * m.scale.z);
		Vector x = m.right * b.x;
		Vector y = m.up * b.y;
		Vector z = m.forward * b.z;
		return x + y + z;
	}
	public static Vector operator / (Vector v, Matrix4x4 m) {
		v -= m.globalPosition;
		return ToLocal(v, m);
	}

	public static Vector ToLocal (Vector v, Matrix4x4 m) {
		Vector r = new Vector (v.x * m.right.x + v.y * m.right.y + v.z * m.right.z,
			v.x * m.up.x + v.y * m.up.y + v.z * m.up.z,
			v.x * m.forward.x + v.y * m.forward.y + v.z * m.forward.z);
		r = new Vector (r.x * m.scale.x, r.y * m.scale.y, r.z * m.scale.z);
		return r;
	}

	public static Vector operator < (Vector v, Matrix4x4 m) {
		return ToLocal (v, m);
	}

    public Matrix ToXNA()
    {
        Matrix m = rotationMatrix;
        m = Matrix.CreateScale(scale) * m;
        m.Translation = globalPosition;
        return m;
    }

	public override string ToString ()
	{
		return string.Format ("[Matrix4x4: parent={0},"
			+ '\n' + " globalEulerAngles={1}, "
			+ '\n' + " localEulerAngles={2}, "
			+ '\n' + " globalPosition={3}, "
			+ '\n' + " scale={4}, localPosition={5}, "
			+ '\n' + " forward={6}, "
			+ '\n' + " up={7}, "
			+ '\n' + " right={8}]", parent, localEulerAngles, localEulerAngles, globalPosition, scale, localPosition, forward, up, right);
	}
}











