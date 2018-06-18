using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Tools;
using xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TQ;

public class IncBehaviour
{
    public IncBehaviour(IncUpdate _update)
    {
        update = _update;
        incBehaviours.Add(this);
    }

    public delegate void IncUpdate ();

    public IncUpdate update { get; private set; }

    public virtual void Update()
    {
        update();
    }

    public static void Destroy(IncBehaviour behaviour)
    {
        incBehaviours.Remove(behaviour);
    }

	public static List<IncBehaviour> incBehaviours
	{
		get {
			return incBehavioursGetter;
		}
		set {
			incBehavioursGetter = value;
		}
	}

	private static List<IncBehaviour> incBehavioursGetter = new List<IncBehaviour> ();

	public static T[] FindObjectsOfType<T> () {
		return incBehaviours.Where ((IncBehaviour arg) => arg is T).Cast<T> ().ToArray ();
	}
}
public struct ColorS
{
	public int r;
	public int g;
	public int b;
	public int a;

	public ColorS (int _r, int _g, int _b) {
		r = _r;
		g = _g;
		b = _b;
		a = 255;
	}
	public ColorS (int _r, int _g, int _b, int _a) {
		r = _r;
		g = _g;
		b = _b;
		a = _a;
	}

	public static implicit operator ColorS (Color a) {
		return new ColorS (a.R, a.G, a.B, a.A);
	}
	public static implicit operator Color (ColorS a) {
		return new Color (new Color(a.r, a.g, a.b), a.a);
	}
}
public struct Vector
{
	public float x;
	public float y;
	public float z;

	public Vector (float _x, float _y, float _z) {
		x = _x;
		y = _y;
		z = _z;
	}
	public Vector (float _x, float _y) {
		x = _x;
		y = _y;
		z = 0;
	}
	public static implicit operator Vector (string s) {
		string[] parts = TextParser.ToWords (s);
		float[] fs = new float[3];
		for (int i = 1; i < parts.Length; i++) {
			fs [i - 1] = TextParser.ParceFloat (parts[i]);
		}
		return new Vector (fs [0], fs [1], fs [2]);
	}

    public static implicit operator Vector(xna.Point s)
    {
		return new Vector (s.X, s.Y);
	}
	public static implicit operator xna.Point (Vector v) {
        return new xna.Point((int)v.x, (int)v.y);
	}

	public static implicit operator xna.Vector4 (Vector v) {
		return new xna.Vector4 (v.x, v.y, v.z, 1);
	}
	public static implicit operator Vector (xna.Vector4 v) {
		return new Vector (v.X, v.Y, v.Z);
	}

	public static implicit operator xna.Vector3 (Vector v) {
		return new xna.Vector3 (v.x, v.y, v.z);
	}
	public static implicit operator Vector (xna.Vector3 v) {
		return new Vector (v.X, v.Y, v.Z);
	}

	public static Vector operator * (Vector a, float b) {
		return new Vector (a.x * b, a.y * b, a.z * b);
	}
	public static Vector operator / (Vector a, float b) {
		return new Vector (a.x / b, a.y / b, a.z / b);
	}
	public static Vector operator + (Vector a, Vector b) {
		return new Vector (a.x + b.x, a.y + b.y, a.z + b.z);
	}
	public static Vector operator - (Vector a, Vector b) {
		return new Vector (a.x - b.x, a.y - b.y, a.z - b.z);
	}
	public static Vector operator - (Vector a) {
		return new Vector (-a.x, -a.y, -a.z);
	}
	public static Vector right
	{
		get {
			return new Vector (1, 0);
		}
	}
	public static Vector left
	{
		get {
			return new Vector (-1, 0);
		}
	}
	public static Vector up
	{
		get {
			return new Vector (0, 1);
		}
	}
	public static Vector one
	{
		get {
			return new Vector (1, 1, 1);
		}
	}
	public static Vector down
	{
		get {
			return new Vector (0, -1);
		}
	}
	public static Vector forward
	{
		get {
			return new Vector (0, 0, 1);
		}
	}
	public static Vector back
	{
		get {
			return new Vector (0, 0, -1);
		}
	}
	public static Vector zero
	{
		get {
			return new Vector (0, 0, 0);
		}
	}
	public float magnitude
	{
		get {
			return (float)Math.Sqrt ((double)(x * x + y * y + z * z));
		}
	}
	public Vector normalized
	{
		get {
			return magnitude > 0 ? new Vector (x, y, z) / magnitude : Vector.zero;
		}
	}
	public override string ToString () {
		return "Vector : [" + x + ", " + y + ", " + z + "]";
	}
}
public class Mathf
{
	public static float Sin (float a) {
		return (float)Math.Sin (a);
	}
    public static float SinDeg(float a)
    {
        a *= Deg2Rad;
        return (float)Math.Sin(a);
    }
	public static float Cos (float a) {
		return (float)Math.Cos (a);
	}
    public static float CosDeg(float a)
    {
        a *= Deg2Rad;
        return (float)Math.Cos(a);
    }
	public const float Deg2Rad = (float)Math.PI / 180f;
	public const float Rad2Deg = 180f / (float)Math.PI;
}
public struct Rect
{

	public Rect (Vector pos, Vector sz) {
		position = pos;
		size = sz;
	}

	public Vector position;
	public Vector size;

	public Vector max
	{
		get {
			return position + size / 2;
		}
	}
	public Vector min
	{
		get {
			return position - size / 2;
		}
	}
	public bool Containts (Vector point) {
		return (point.x < max.x && point.y < max.y && point.x >= min.x && point.y >= min.y);
	}
	public bool Containts (Vector point, float radius) {return (position + size / 2 - point).magnitude < radius;
	}
}
public class Input
{
    public static Vector arrows
    {
        get
        {
            KeyboardState state = Keyboard.GetState();
            Vector v = Vector.zero;
            if (state.IsKeyDown(Keys.A)) v += Vector.right;
            if (state.IsKeyDown(Keys.W)) v += Vector.forward;
            if (state.IsKeyDown(Keys.S)) v += Vector.back;
            if (state.IsKeyDown(Keys.D)) v += Vector.left;
            if (state.IsKeyDown(Keys.E)) v += Vector.up;
            if (state.IsKeyDown(Keys.Q)) v += Vector.down;

            return v.normalized;
        }
    }

    public static bool IsKeyPressed(Keys key)
    {
        return Keyboard.GetState().IsKeyDown(key);
    }

    public static Vector mouse
    {
        get
        {
            Vector v = Vector.zero;
            MouseState state = Mouse.GetState();
            Vector pos = new Vector(state.X, state.Y);
            xna.Rectangle rect = Program.currentEngine.Window.ClientBounds;
            Vector last = rect.Center;
            v = pos - last;
            Mouse.SetPosition((int)last.x, (int)last.y);
            return v;
        }
    }
}
public class Resources
{
}