using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using Unity.Mathematics;

public class BasicFunctions : MonoBehaviour
{

	/*
	#region FindObject
	public static GameObject FindObject ( string _string )
	{
		GameObject oReturn = null;
		GameObject[] oFound = GameObject.FindGameObjectsWithTag(_string);

		if ( oFound.Length > 0 )
		{
			foreach ( GameObject o in oFound )
			{
				//
			}
		}

		return oReturn;
	}
	#endregion
	*/

	//public static Vector3 ClampMagnitudeMinMax(Vector3 v, float max, float min)
	//{
	//    double sm = v.sqrMagnitude;
	//    if (sm > (double)max * (double)max) return v.normalized * max;
	//    else if (sm < (double)min * (double)min) return v.normalized * min;
	//    return v;
	//}

	public static bool ScreenPointInRectTransform(Vector2 _screenPoint, RectTransform _rectTransform)
	{
		Vector2 localScreenPos = _rectTransform.InverseTransformPoint(_screenPoint);
		return _rectTransform.rect.Contains(localScreenPos);
	}

	public static void NormalizeQuaternion(ref Quaternion q)
    {
        float sum = 0;
        for (int i = 0; i < 4; ++i)
            sum += q[i] * q[i];
        float magnitudeInverse = 1 / Mathf.Sqrt(sum);
        for (int i = 0; i < 4; ++i)
            q[i] *= magnitudeInverse;
    }

    public static bool QuaternionIsNaN ( Quaternion _q )
    {
        return float.IsNaN(_q.x) || float.IsNaN(_q.y) || float.IsNaN(_q.z) || float.IsNaN(_q.w);
    }

    public static Bounds CalculateLocalBounds ( Transform _transform )
    {
        //if ( _transform.GetComponent<Renderer>() != null )
        //{
            Quaternion currentRotation = _transform.rotation;
            _transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Bounds bounds = new Bounds(_transform.position, Vector3.zero);

            //foreach ( Renderer renderer in _transform.GetComponentsInChildren<Renderer>() )
            //{
            //    bounds.Encapsulate(renderer.bounds);
            //}

            bounds = _transform.GetComponent<Renderer>().bounds;

            Vector3 localCenter = bounds.center - _transform.position;
            bounds.center = localCenter;
            //Debug.Log("The local bounds of this model is " + bounds);

            _transform.rotation = currentRotation;

            return bounds;
        //}

        //return null;
    }

    public static void AddDescendantsWithRenderer(Transform _parent, List<GameObject> _objectList, List<Renderer> _rendererList)
    {
        foreach (Transform child in _parent)
        {
            if (child.gameObject.activeSelf)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    Renderer r = child.GetComponent<Renderer>();
                    if ( r != null && r.enabled )
                    {
                        MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                        _objectList.Add(child.gameObject);
                        _rendererList.Add(r);
                    }

                    // debug
                    //Debug.Log("stored " + child.name + " to a boundsOctree || " + Time.time.ToString());
                }
                AddDescendantsWithRenderer(child, _objectList, _rendererList);
            }
        }
    }

    public static void AddDescendantsWithMeshFilter ( Transform _parent, List<GameObject> _objectList, List<Bounds> _boundList )
    {
        foreach ( Transform child in _parent )
        {
            if ( child.gameObject.activeSelf )
            {
                if ( child.GetComponent<MeshFilter>() != null )
                {
                    MeshFilter meshFilter = child.GetComponent<MeshFilter>();
                    _objectList.Add(child.gameObject);
                    _boundList.Add(meshFilter.sharedMesh.bounds);

                    // debug
                    //Debug.Log("stored " + child.name + " to a boundsOctree || " + Time.time.ToString());
                }
                AddDescendantsWithMeshFilter(child,_objectList,_boundList);
            }
        }
    }

    public static void AddDescendants ( Transform _parent, List<GameObject> _list )
    {
        foreach ( Transform child in _parent )
        {
            _list.Add(child.gameObject);
            AddDescendants(child,_list);
        }
    }

    public static void AddDescendantsWithTag ( Transform _parent, string _tag, List<GameObject> _list )
    {
        foreach ( Transform child in _parent )
        {
            if ( child.CompareTag(_tag) )
            {
                _list.Add(child.gameObject);
            }
            AddDescendantsWithTag(child,_tag,_list);
        }
    }

    public static void DrawMeshInstanceNoLimit(Mesh mesh, Material material, Matrix4x4[] matrices)
    {
        int startIndex = 0;
        int limit = 1023;

        //System.DateTime start = System.DateTime.Now;

        while (startIndex < matrices.Length)
        {
            int endIndex = Mathf.Min(matrices.Length, startIndex + limit);
            int arraySize = endIndex - startIndex;

            Matrix4x4[] splitMatrices = new Matrix4x4[arraySize];

            int j = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                splitMatrices[j] = matrices[i];
                j++;
            }

            startIndex += arraySize;

            Graphics.DrawMeshInstanced(mesh, 0, material, splitMatrices);

            //if (System.DateTime.Now - start > System.TimeSpan.FromSeconds(5))
                //break;
        }

    }

    public struct TBool
    {
        readonly byte _value;
        public TBool(bool value) { _value = (byte)(value ? 1 : 0); }
        public static implicit operator TBool(bool value) { return new TBool(value); }
        public static implicit operator bool(TBool value) { return value._value != 0; }
    }

    #region SetLayerRecursively
    public static void SetLayerRecursively ( Transform _transform, int _layer )
    {
        _transform.gameObject.layer = _layer;
        foreach ( Transform child in _transform )
        {
            child.gameObject.layer = _layer;
            if ( child.childCount > 0 )
            {
                SetLayerRecursively(child.transform,_layer);
            }
        }
    }
    #endregion

    //#region TranslateMatrix
    ///// <summary>Returns a float4x4 translation matrix given a float3 translation vector.</summary>
    //public static Unity.Mathematics.float4x4 TranslateMatrix(Unity.Mathematics.float3 vector)
    //{
    //    return new Unity.Mathematics.float4x4(new Unity.Mathematics.float4(1.0f, 0.0f, 0.0f, 0.0f),
    //                        new Unity.Mathematics.float4(0.0f, 1.0f, 0.0f, 0.0f),
    //                        new Unity.Mathematics.float4(0.0f, 0.0f, 1.0f, 0.0f),
    //                        new Unity.Mathematics.float4(vector.x, vector.y, vector.z, 1.0f));
    //}
    //#endregion

    /*
    #region SpringVector
    public static Vector3 SpringVector ( Vector3 _add, Vector3 _from, Vector3 _to, float _dst, float _amount, float _mass, float _fric )
    {
        Vector3 ret = _add;

        Vector3 off = (_from - _to);
        float dist = (off.magnitude - _dst);
        if ( dist != 0f )
        {
            Vector2 angle_xy = new Vector2(off.x,off.y);

            float ptDst = angle_xy.magnitude;
            Vector2 angle_z = new Vector2(ptDst,off.z);

            off.x = lengthdir_x(lengthdir_x(dist,angle_z),angle_xy); 
            off.y = lengthdir_y(lengthdir_x(dist,angle_z),angle_xy); 
            off.z = lengthdir_y(dist,angle_z); 

            float r1 = _mass;

            ret.x -= (off.x * r1) * _amount;
            ret.y -= (off.y * r1) * _amount;
            ret.z -= (off.z * r1) * _amount;

            ret.x *= _fric;
            ret.y *= _fric;
            ret.z *= _fric;
        }

        return ret;
    }
    #endregion
    */


    #region SpringVector
    public static Vector3 SpringVector(Vector3 _add, Vector3 _from, Vector3 _to, float _dst, float _amount, float _mass, float _fric)
    {
        Vector3 ret = _add;

        Vector3 off = (_from - _to);
        float dist = (off.magnitude - _dst);
        if (dist != 0f)
        {
            Vector2 angle_xy = new Vector2(off.x, off.y);

            float ptDst = angle_xy.magnitude;
            if (ptDst != 0f)
            {
                angle_xy /= ptDst;
            }

            Vector2 angle_z = new Vector2(ptDst, off.z).normalized;

            off.x = lengthdir_x_notNormalize(lengthdir_x_notNormalize(dist, angle_z), angle_xy);
            off.y = lengthdir_y_notNormalize(lengthdir_x_notNormalize(dist, angle_z), angle_xy);
            off.z = lengthdir_y_notNormalize(dist, angle_z);

            float r1 = _mass;

            ret.x -= (off.x * r1) * _amount;
            ret.y -= (off.y * r1) * _amount;
            ret.z -= (off.z * r1) * _amount;

            ret.x *= _fric;
            ret.y *= _fric;
            ret.z *= _fric;
        }

        return ret;
    }
    #endregion

    #region SpringVectorDeltaTime
    public static Vector3 SpringVectorDeltaTime(Vector3 _add, Vector3 _from, Vector3 _to, float _dst, float _amount, float _mass, float _fric, float _deltaTime)
    {
        Vector3 ret = _add;

        Vector3 off = (_from - _to);
        float dist = (off.magnitude - _dst);
        if (dist != 0f)
        {
            Vector2 angle_xy = new Vector2(off.x, off.y);

            float ptDst = angle_xy.magnitude;
            Vector2 angle_z = new Vector2(ptDst, off.z);

            off.x = lengthdir_x(lengthdir_x(dist, angle_z), angle_xy);
            off.y = lengthdir_y(lengthdir_x(dist, angle_z), angle_xy);
            off.z = lengthdir_y(dist, angle_z);

            float r1 = _mass;
            ret.x -= ((off.x * r1) * _amount) * _deltaTime;
            ret.y -= ((off.y * r1) * _amount) * _deltaTime;
            ret.z -= ((off.z * r1) * _amount) * _deltaTime;

            float deltaFric = _fric;
            ret.x *= deltaFric;
            ret.y *= deltaFric;
            ret.z *= deltaFric;
        }

        return ret;
    }
    #endregion

    public static float FastSqrtInvAroundOne ( float x )
    {
        float a0 = 15f / 8f;
        float a1 = -5f / 4f;
        float a2 = 3f / 8f;

        return a0 + a1 * x + a2 * x * x;
    }

    public static Vector3 FastNormalize3D ( Vector3 _v )
    {
        float len_sq = _v.x * _v.x + _v.y * _v.y + _v.z * _v.z;
        float len_inv = FastSqrtInvAroundOne(len_sq);

        _v.x *= len_inv;
        _v.y *= len_inv;
        _v.z *= len_inv;

        return _v;
    }

    public static Vector2 FastNormalize2D ( Vector2 _v )
    {
        float len_sq = _v.x * _v.x + _v.y * _v.y;
        float len_inv = FastSqrtInvAroundOne(len_sq);

        _v.x *= len_inv;
        _v.y *= len_inv;

        return _v;
    }

    #region lengthdir_magnitude
    public static float lengthdir_magnitude ( float _len, float _dir ) 
    {
        _dir = _dir * _len;
        return _dir;
    }
    #endregion

    #region lengthdir_x
    public static float lengthdir_x ( float _len, Vector2 _dir ) 
    {
        _dir.x = _dir.normalized.x * _len;
        return _dir.x;
    }
    #endregion

    #region lengthdir_y
    public static float lengthdir_y ( float _len, Vector2 _dir ) 
    {
        _dir.y = _dir.normalized.y * _len;
        return _dir.y;
    }
    #endregion

    #region lengthdir_x_notNormalize
    public static float lengthdir_x_notNormalize(float _len, Vector2 _dir)
    {
        _dir.x = _dir.x * _len;
        return _dir.x;
    }
    #endregion

    #region lengthdir_y_notNormalize
    public static float lengthdir_y_notNormalize(float _len, Vector2 _dir)
    {
        _dir.y = _dir.y * _len;
        return _dir.y;
    }
    #endregion

    #region ClampMagnitudeMinMax
    public static Vector3 ClampMagnitudeMinMax(Vector3 v, float min, float max)
    {
        double sm = v.sqrMagnitude;
        if(sm > (double)max * (double)max) return v.normalized * max;
        else if(sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }
    #endregion

    #region AngleDir
    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 right = Vector3.Cross(up, fwd);        // right vector
        float dir = Vector3.Dot(right, targetDir);

        if (dir > 0f) {
            return 1f;
        } else if (dir < 0f) {
            return -1f;
        } else {
            return 0f;
        }
    }
    #endregion

    #region AngleDirReal
    public static float AngleDirReal(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 right = Vector3.Cross(up, fwd);        // right vector
        float dir = Vector3.Dot(right, targetDir);

        return dir;
    }
    #endregion

    #region AngleDirInt
    public static int AngleDirInt(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 right = Vector3.Cross(up, fwd);        // right vector
        float dir = Vector3.Dot(right, targetDir);

        if (dir > 0f)
        {
            return 1;
        }
        else if (dir < 0f)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    #endregion

    #region DistanceLineSegmentPoint
    // Distance to point (p) from line segment (end points a b)
    public static float DistanceLineSegmentPoint( Vector3 a, Vector3 b, Vector3 p )
    {
        // If a == b line segment is a point and will cause a divide by zero in the line segment test.
        // Instead return distance from a
        if (a == b)
            return Vector3.Distance(a, p);

        // Line segment to point distance equation
        Vector3 ba = b - a;
        Vector3 pa = a - p;
        return (pa - ba * (Vector3.Dot(pa, ba) / Vector3.Dot(ba, ba))).magnitude;
    }
    #endregion

	#region IsApproximately
	public static bool IsApproximately ( float _a, float _b, float _tolerance )
	{
		float readValue = (_a - _b);
		float absedValue = ( readValue < 0f ) ? -readValue : readValue;
		return ( absedValue < _tolerance );
	}
	#endregion

	#region GetNormal
	public static Vector3 GetNormal ( Vector3 _a, Vector3 _b, Vector3 _c ) 
	{
		Vector3 side1 = _b - _a;
		Vector3 side2 = _c - _a;

		return Vector3.Cross(side1,side2).normalized;
	}
	#endregion

	#region LerpByDistance 
	public static Vector3 LerpByDistance ( Vector3 _a, Vector3 _b, float _x )
	{
		return (_x * (_b - _a).normalized) + _a;
	}
    #endregion

	#region IsVisibleFrom
	public static bool IsVisibleFrom ( Renderer _renderer, Camera _cam )
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_cam);

		return GeometryUtility.TestPlanesAABB(planes,_renderer.bounds);
	}
	#endregion

    #region Wrap string
    protected const string _newline = "\r\n";

    public static string WrapString (string _string, int _width)
    {
        int pos, next;
        StringBuilder sb = new StringBuilder();

        // Lucidity check
        if (_width < 1)
            return _string;

        // Parse each line of text
        for (pos = 0; pos < _string.Length; pos = next)
        {
            // Find end of line
            int eol = _string.IndexOf(_newline, pos);

            if (eol == -1)
                next = eol = _string.Length;
            else
                next = eol + _newline.Length;

            // Copy this line of text, breaking into smaller lines as needed
            if (eol > pos)
            {
                do
                {
                    int len = eol - pos;

                    if (len > _width)
                        len = BreakLine(_string, pos, _width);

                    sb.Append(_string, pos, len);
                    sb.Append(_newline);

                    // Trim whitespace following break
                    pos += len;

                    while (pos < eol && char.IsWhiteSpace(_string[pos]))
                        pos++;

                } while (eol > pos);
            }
            else
            {
                sb.Append(_newline); // Empty line
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Locates position to break the given line so as to avoid
    /// breaking words.
    /// </summary>
    /// <param name="text">String that contains line of text</param>
    /// <param name="pos">Index where line of text starts</param>
    /// <param name="max">Maximum line length</param>
    /// <returns>The modified line length</returns>
    public static int BreakLine(string text, int pos, int max)
    {
        // Find last whitespace in line
        int i = max - 1;
        while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
            i--;
        if (i < 0)
            return max; // No whitespace found; break at maximum length
                        // Find start of whitespace
        while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
            i--;
        // Return length of text before whitespace
        return i + 1;
    }
    #endregion

    #region PickRandomFromList -- string
    public static string PickRandomFromList(List<string> _list)
    {
        string strRet = "";

        int rIndex = Random.Range(1,_list.Count);
        strRet = _list[rIndex];
        _list[rIndex] = _list[0];
        _list[0] = strRet;

        return strRet;
    }
    #endregion

	#region PickRandomAudioClipFromArray
	public static AudioClip PickRandomAudioClipFromArray ( AudioClip[] _array )
	{
		AudioClip ret = null;

		if ( _array != null && _array.Length > 0 )
		{
			if ( _array.Length > 1 )
			{
				int rIndex = Random.Range(1,_array.Length - 1);
				ret = _array[rIndex];
				_array[rIndex] = _array[0];
				_array[0] = ret;
			}
			else
			{
				ret = _array[0];
			}
		}

		return ret;
	}
	#endregion

	#region PickRandomAudioClipFromList
	public static AudioClip PickRandomAudioClipFromList ( List<AudioClip> _list )
	{
		AudioClip ret = null;

		if ( _list != null )
		{
			if ( _list.Count > 1 )
			{
				int rIndex = Random.Range(1,_list.Count);
				ret = _list[rIndex];
				_list[rIndex] = _list[0];
				_list[0] = ret;
			}
			else
			{
				ret = _list[0];
			}
		}
		
		return ret;
	}
	#endregion

	#region PickRandomMeshFromArray
	public static Mesh PickRandomMeshFromArray ( Mesh[] _array )
	{
		Mesh ret = null;

		if ( _array != null )
		{
			if ( _array.Length > 1 )
			{
				int rIndex = Random.Range(1,_array.Length - 1);
				ret = _array[rIndex];
				_array[rIndex] = _array[0];
				_array[0] = ret;
			}
			else
			{
				ret = _array[0];
			}
		}

		return ret;
	}
	#endregion

    #region PickRandomMaterialFromArray
    public static Material PickRandomMaterialFromArray ( Material[] _array )
    {
        Material ret = null;

        if ( _array != null )
        {
            if ( _array.Length > 1 )
            {
                int rIndex = Random.Range(1,_array.Length - 1);
                ret = _array[rIndex];
                _array[rIndex] = _array[0];
                _array[0] = ret;
            }
            else
            {
                ret = _array[0];
            }
        }

        return ret;
    }
    #endregion

    #region PickRandomObjectFromArray
    public static GameObject PickRandomObjectFromArray ( GameObject[] _array )
    {
        GameObject ret = null;

        if ( _array != null )
        {
            if ( _array.Length > 1 )
            {
                int rIndex = Random.Range(1,_array.Length - 1);
                ret = _array[rIndex];
                _array[rIndex] = _array[0];
                _array[0] = ret;
            }
            else
            {
                ret = _array[0];
            }
        }

        return ret;
    }
    #endregion

    #region PickRandomMeshFromList
    public static Mesh PickRandomMeshFromList ( List<Mesh> _list )
    {
        Mesh ret = null;

		if ( _list != null )
		{
			if ( _list.Count > 1 )
			{
	        	int rIndex = Random.Range(1,_list.Count);
				ret = _list[rIndex];
	        	_list[rIndex] = _list[0];
				_list[0] = ret;
			}
			else
			{
				ret = _list[0];
			}
		}

		return ret;
    }
    #endregion

    #region Check if a string starts with a vowel
    public static bool StartsWithVowel ( string _checkString )
    {
        return ( _checkString.StartsWith("e") || _checkString.StartsWith("a") || _checkString.StartsWith("i") || _checkString.StartsWith("o") || _checkString.StartsWith("u") || _checkString.StartsWith("E") || _checkString.StartsWith("A") || _checkString.StartsWith("I") || _checkString.StartsWith("O") || _checkString.StartsWith("U") );
    }
    #endregion

    #region Shuffle
    public static void Shuffle<T> (T[] array, System.Random _rand)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(_rand.NextDouble() * (n - i));
            T t = array[r];
            array[r] = array[i];
            array[i] = t;
        }
    }
    #endregion

	#region ConvertRange
	public static float ConvertRange ( float _value, float _fromMin, float _fromMax, float _toMin, float _toMax )
	{
		_value = Mathf.Clamp(_value,_fromMin,_fromMax);

		float rangeA = _fromMax - _fromMin;
		_value = (_value - _fromMin) / rangeA;

		float rangeB = _toMax - _toMin;
		_value = _toMin + (_value * (rangeB / 1f));

		return _value;
	}
	#endregion

	#region Take Screenshot
	public static Texture2D TakeScreenshot ( int _width, int _height, Camera _screenshotCamera )
	{
		if ( _width <= 0 || _height <= 0 ) return null;
		if ( _screenshotCamera == null ) _screenshotCamera = Camera.main;

		Texture2D screenshot = new Texture2D(_width,_height,TextureFormat.RGB24,false);
		RenderTexture renderTex = new RenderTexture(_width,_height,24);
		_screenshotCamera.targetTexture = renderTex;
		_screenshotCamera.Render();
		RenderTexture.active = renderTex;
		screenshot.ReadPixels(new Rect(0,0,_width,_height),0,0);
		screenshot.Apply(false);
		_screenshotCamera.targetTexture = null;
		RenderTexture.active = null;
		Destroy(renderTex);

		return screenshot;
	}
	#endregion

	#region Color to Hex
	public static string ColorToHex ( Color32 _color, bool _includeAlpha )
	{
		System.String rs = System.Convert.ToString(_color.r,16).ToUpper();
		System.String gs = System.Convert.ToString(_color.g,16).ToUpper();
		System.String bs = System.Convert.ToString(_color.b,16).ToUpper();
		System.String a_s = System.Convert.ToString(_color.a,16).ToUpper();

		while (rs.Length < 2) rs = "0" + rs;
		while (gs.Length < 2) gs = "0" + gs;
		while (bs.Length < 2) bs = "0" + bs;
		while (a_s.Length < 2) a_s = "0" + a_s;

		if ( _includeAlpha ) return "#" + rs + gs + bs + a_s;
		return "#" + rs + gs + bs;
	}
	#endregion

	#region Hex to color
	public static Color HexToColor ( string _hex )
	{
		Color col = new Color(0,0,0,0);

		if ( _hex != null && _hex.Length > 0 )
		{
			string str = _hex.Substring(1,_hex.Length - 1);

			col.r = (float)System.Int32.Parse(str.Substring(0,2),NumberStyles.AllowHexSpecifier) / 255f;
			col.g = (float)System.Int32.Parse(str.Substring(2,2),NumberStyles.AllowHexSpecifier) / 255f;
			col.b = (float)System.Int32.Parse(str.Substring(4,2),NumberStyles.AllowHexSpecifier) / 255f;

			if ( str.Length == 8 ) 
				col.a = (float)System.Int32.Parse(str.Substring(6,2),NumberStyles.AllowHexSpecifier) / 255f;
			else
				col.a = 1f;
		}

		return col;
	}
	#endregion

	#region Convert color to HSB
	public static Vector3 ColorToHSB ( Color _color ) 
	{
		float minValue = Mathf.Min(_color.r,Mathf.Min(_color.g,_color.b));
		float maxValue = Mathf.Max(_color.r,Mathf.Max(_color.g,_color.b));
		float delta = maxValue - minValue;
		float h = 0f;
		float s = 0f;
		float b = maxValue;
		
		// # Calculate the hue (in degrees of a circle, between 0 and 360)
		if ( maxValue == _color.r ) 
		{
			if ( _color.g >= _color.b ) 
			{
				if ( delta == 0f ) 
					h = 0f;
				else 
					h = 60f * (_color.g - _color.b) / delta;
			} 
			else if ( _color.g < _color.b ) 
			{
				h = 60f * ( _color.g - _color.b ) / delta + 360f;
			}
		} 
		else if ( maxValue == _color.g ) 
		{
			h = 60f * ( _color.b - _color.r ) / delta + 120f;
		} 
		else if ( maxValue == _color.b ) 
		{
			h = 60f * ( _color.r - _color.g ) / delta + 240f;
		}
		
		// Calculate the saturation (between 0 and 1)
		if ( maxValue == 0 ) 
			s = 0f;
		else 
			s = 1f - (minValue / maxValue);

		return new Vector3(h / 360f,s,b);
	}
	#endregion

	#region Convert HSB to color
	public static Color HSBToColor ( Vector4 _hsba ) 
	{
		// When saturation = 0, then r, g, b represent grey value (= brightness (z)).
		float r = _hsba.z;
		float g = _hsba.z;
		float b = _hsba.z;

		if ( _hsba.y > 0f ) 
		{
			// Calc sector
			float secPos = (_hsba.x * 360f) / 60f;
			int secNr = Mathf.FloorToInt(secPos);
			float secPortion = secPos - secNr;
			
			// Calc axes p, q and t
			float p = _hsba.z * (1f - _hsba.y);
			float q = _hsba.z * (1f - (_hsba.y * secPortion));
			float t = _hsba.z * (1f - (_hsba.y * (1f - secPortion)));
			
			// Calc rgb
			if ( secNr == 1 ) 
			{
				r = q;
				g = _hsba.z;
				b = p;
			} 
			else if ( secNr == 2 ) 
			{
				r = p;
				g = _hsba.z;
				b = t;
			} 
			else if ( secNr == 3 ) 
			{
				r = p;
				g = q;
				b = _hsba.z;
			} 
			else if ( secNr == 4 ) 
			{
				r = t;
				g = p;
				b = _hsba.z;
			} 
			else if ( secNr == 5 ) 
			{
				r = _hsba.z;
				g = p;
				b = q;
			} 
			else 
			{
				r = _hsba.z;
				g = t;
				b = p;
			}
		}

		return new Color(r,g,b,_hsba.w);
	}
	#endregion

	#region Convert angle to 0-360 angle
	public static float To360Anglefloat ( float _angle ) 
	{
		while ( _angle < 0f ) _angle += 360f;
		while ( _angle >= 360f ) _angle -= 360f;

		return _angle;
	}
	#endregion

	#region Convert vector of 3 angles to 0-360 angle
	public static Vector3 To360AngleVector3 ( Vector3 _angles )
	{
		_angles.x = To360Anglefloat(_angles.x);
		_angles.y = To360Anglefloat(_angles.y);
		_angles.z = To360Anglefloat(_angles.z);

		return _angles;
	}
	#endregion

	#region Convert angle to -180 - 180 angle
	public static float To180Anglefloat ( float _angle ) 
	{
		while ( _angle < -180f ) _angle += 360f;
		while ( _angle >= 180f ) _angle -= 360f;
		
		return _angle;
	}
	#endregion
	
	#region Convert vector of 3 angles to -180 - 180 angle
	public static Vector3 To180AngleVector3 ( Vector3 _angles )
	{
		_angles.x = To180Anglefloat(_angles.x);
		_angles.y = To180Anglefloat(_angles.y);
		_angles.z = To180Anglefloat(_angles.z);
		
		return _angles;
	}
	#endregion

	#region Convert math angle to compass angle 
	public static float MathAngleToCompassAngle ( float _angle )
	{
		_angle = 90f - _angle;

		return To360Anglefloat(_angle);
	}
	#endregion

	#region Lerp compass angle
	public static float CompassAngleLerp ( float _from, float _to, float _portion )
	{
		float dif = To180Anglefloat(_to - _from);
		dif *= Mathf.Clamp01(_portion);

		return To360Anglefloat(_from + dif);
	}
	#endregion

	#region Create an empty texture with a single color
	public static Texture2D CreateEmptyTexture ( int _w, int _h, Color _color )
	{
		Texture2D img = new Texture2D(_w,_h,TextureFormat.RGBA32,false);
		Color[] pixels = img.GetPixels(0);

		for ( int i = 0; i < pixels.Length; i ++ )
		{
			pixels[i] = _color;
		}

		img.SetPixels(pixels,0);
		img.Apply();

		return img;
	}
	#endregion

	#region Change texture color
	public static Texture2D ChangeTextureColor ( Texture2D _originalTexture, float _deltaHue, float _deltaSaturation, float _deltaBrightness ) 
	{
		Texture2D newTexture = new Texture2D(_originalTexture.width,_originalTexture.height,TextureFormat.RGBA32,false);
		Color[] originalPixels = _originalTexture.GetPixels(0);
		Color[] newPixels = newTexture.GetPixels(0);

		for ( int i = 0; i < originalPixels.Length; i ++ ) 
		{
			Vector4 hsba = ColorToHSB(originalPixels[i]);
			hsba.x += _deltaHue;
			hsba.y += _deltaSaturation;
			hsba.z += _deltaBrightness;
			newPixels[i] = HSBToColor(hsba);
		}

		newTexture.SetPixels(newPixels,0);
		newTexture.Apply();

		return newTexture;
	}
	#endregion

	#region Change texture contrast
	public static Texture2D ChangeTextureContrastLinear ( Texture2D _originalTexture, float _contrast, float _power ) 
	{
		if ( _power < 0f ) _power = 1f;
		Texture2D newTexture = new Texture2D(_originalTexture.width,_originalTexture.height,TextureFormat.RGBA32, false);
		Color[] originalPixels = _originalTexture.GetPixels(0);
		Color[] newPixels = newTexture.GetPixels(0);
		float avgGrey = new float();

		for ( int i = 0; i < originalPixels.Length; i ++ ) 
		{
			Color c = originalPixels[i];
			avgGrey += c.r;
			avgGrey += c.g;
			avgGrey += c.b;
		}

		avgGrey = avgGrey / (3f * originalPixels.Length);
		
		for ( int i = 0; i < originalPixels.Length; i ++ ) 
		{
			Color c = originalPixels[i];
			float deltaR = c.r - avgGrey;
			float deltaG = c.g - avgGrey;
			float deltaB = c.b - avgGrey;
			newPixels[i] = new Color(avgGrey + (deltaR * _contrast),avgGrey + (deltaG * _contrast),avgGrey + (deltaB * _contrast),c.a);
		}

		newTexture.SetPixels(newPixels,0);
		newTexture.Apply();

		return newTexture;
	}
	#endregion

	#region Crop texture
	public static Texture2D CropTexture ( Texture2D _originalTexture, Rect _cropRect ) 
	{
		// Make sure the crop rectangle stays within the original Texture dimensions
		_cropRect.x = Mathf.Clamp(_cropRect.x,0,_originalTexture.width);
		_cropRect.width = Mathf.Clamp(_cropRect.width,0,_originalTexture.width - _cropRect.x);
		_cropRect.y = Mathf.Clamp(_cropRect.y,0,_originalTexture.height);
		_cropRect.height = Mathf.Clamp(_cropRect.height,0,_originalTexture.height - _cropRect.y);

		if ( _cropRect.height <= 0 || _cropRect.width <= 0 ) 
			return null; // dont create a Texture with size 0
		
		Texture2D newTexture = new Texture2D((int)_cropRect.width,(int)_cropRect.height,TextureFormat.RGBA32,false);
		Color[] pixels = _originalTexture.GetPixels((int)_cropRect.x,(int)_cropRect.y,(int)_cropRect.width,(int)_cropRect.height,0);
		newTexture.SetPixels(pixels);
		newTexture.Apply();

		return newTexture;
	}
	#endregion

	#region Mirror texture
	public static Texture2D MirrorTexture ( Texture2D _originalTexture, bool _horizontal, bool _vertical ) 
	{
		Texture2D newTexture = new Texture2D(_originalTexture.width,_originalTexture.height,TextureFormat.RGBA32,false);;
		Color[] originalPixels = _originalTexture.GetPixels(0);
		Color[] newPixels = newTexture.GetPixels(0);

		for ( int y = 0; y < _originalTexture.height; y ++ ) 
		{
			for ( int x = 0; x < _originalTexture.width; x ++ ) 
			{
				int newX = _horizontal ? ( newTexture.width - 1 - x) : x;
				int newY = _vertical ? ( newTexture.height - 1 - y ) : y;
				newPixels[(newY * newTexture.width) + newX] = originalPixels[(y * _originalTexture.width) + x];
			}
		}

		newTexture.SetPixels(newPixels,0);
		newTexture.Apply();

		return newTexture;
	}
	#endregion

	#region Convert string to int
	public static int StringToInt ( string _string ) 
	{
		int parsedInt = 0;
		if ( _string != null && int.TryParse(_string,out parsedInt) ) return parsedInt;

		return 0;
	}
	#endregion

	#region Convert string to float
	public static float StringTofloat ( string _string ) 
	{
		float parsedfloat = 0f;
		if ( _string != null && float.TryParse(_string,out parsedfloat) ) return parsedfloat;

		return 0f;
	}
	#endregion

	#region Convert string to vector3
	public static Vector3 StringToVector3 ( string _string ) 
	{
		Vector3 v = new Vector3(0,0,0);
		if ( _string != null && _string.Length > 0 ) 
		{
			if ( _string.IndexOf(",",0) >= 0 ) 
			{
				int p0 = 0;
				int p1 = 0;
				int c = 0;
				p1 = _string.IndexOf(",",p0);
				while ( p1 > p0 && c <= 3) 
				{
					v[c ++] = float.Parse(_string.Substring(p0,p1 - p0));
					p0 = p1 + 1;
					if ( p0 < _string.Length ) p1 = _string.IndexOf(",",p0);
					if ( p1 < 0 ) p1 = _string.Length;
				}
			}
		}

		return v;
	}
	#endregion

	#region Convert float to string
	public static string floatToString ( float _float, int _decimals ) 
	{
		if ( _decimals <= 0 ) return "" + Mathf.RoundToInt(_float);
		string format = "{0:F" + _decimals + "}";

		return string.Format(format,_float);
	}
	#endregion

	#region Convert vector3 to string
	public static string Vector3ToString ( Vector3 _vector, int _decimals ) 
	{
		if ( _decimals <= 0 ) return "<" + Mathf.RoundToInt(_vector.x) + "," + Mathf.RoundToInt(_vector.y) + "," + Mathf.RoundToInt(_vector.z) + ">";
		string format = "{0:F" + _decimals + "}";

		return "<" + string.Format(format,_vector.x) + "," + string.Format(format,_vector.y) + "," + string.Format(format,_vector.z) + ">";
	}
    #endregion

    #region Pick a random point within a radius
    public static Vector3 PickPointWithinRadius ( Vector3 _origin, float _rMin, float _rMax )
    {
        Vector3 rPoint = _origin + Random.insideUnitSphere * Random.Range(_rMin, _rMax);
        rPoint.y = _origin.y;

        return rPoint;
    }
    #endregion

    #region Rotate towards a point
    public static void RotateTowardsPoint (Transform _applyTo, Vector3 _from, Vector3 _to, float _speed, bool _freeRotation, Vector3 _extraOffset )
    {
        Vector3 lookVector = (_to - _from);
        lookVector.y = _from.y;

		Quaternion lookDir = Quaternion.LookRotation(lookVector) * Quaternion.Euler(_extraOffset);

		if ( !_freeRotation )
		{
	        lookDir.x = 0f;
	        lookDir.z = 0f;
		}

        _applyTo.rotation = Quaternion.Slerp(_applyTo.rotation,lookDir,_speed);
    }
    #endregion

	#region Rotate towards a point - complex
	public static void RotateTowardsPointComplex (Transform _applyTo, Vector3 _from, Vector3 _to, float _speed, bool _freeRotation, Vector3 _extraOffset )
	{
		Vector3 lookVector = (_to - _from);
		
		Quaternion lookDir = Quaternion.LookRotation(lookVector) * Quaternion.Euler(_extraOffset);
		
		if ( !_freeRotation )
		{
			lookDir.x = 0f;
			lookDir.z = 0f;
		}
		
		_applyTo.localRotation = Quaternion.Slerp(_applyTo.localRotation,lookDir,_speed);
	}
	#endregion

	/*
    #region Pick random direction
    public static Vector3 PickRandomDirection ( Vector3 _origin, float _minAngle, float _maxAngle, float _minDist, float _maxDist, bool _checkForCollision, LayerMask _collisionMask )
    {
        Vector3 retVector = Vector3.zero;

        float rAngle = Random.Range(_minAngle, _maxAngle);
        float rDist = Random.Range(_minDist, _maxDist);

        // Start
        Vector3 rStart = _origin;

        // End
        Vector3 rPoint = Random.insideUnitSphere * rDist;
        Vector3 rEnd = rStart + new Vector3(rPoint.x, 0f, rPoint.z);

        // Direction
        Vector3 rDir = (rEnd - rStart);

        RaycastHit hit;
        if ( _checkForCollision )
        {
            if ( Physics.Linecast(rStart,rEnd,out hit,_collisionMask) )
            {
                Vector3 targetedPoint = hit.point - (rDir.normalized * .5f);

                if ( Vector3.Distance(rStart, targetedPoint) > _minDist )
                    retVector = targetedPoint;
            }
            else
            {
                retVector = rEnd;
            }
        }
        else
        {
            retVector = rEnd;
        }

        return retVector;
    }
    #endregion
	*/

    #region Make a smooth curve
    public static Vector3[] MakeSmoothCurve ( Vector3[] _arrayToCurve, float _smoothness )
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        _smoothness = Mathf.Clamp(_smoothness,1f,10f);

        pointsLength = _arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(_smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0f;
        for ( int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve ++ )
        {
            t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);

            points = new List<Vector3>(_arrayToCurve);

            for ( int j = pointsLength - 1; j > 0; j -- )
            {
                for ( int i = 0; i < j; i ++ )
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }
    #endregion

	#region Make a smooth curve - List
	public static List<Vector3> MakeSmoothCurveList ( List<Vector3> _listToCurve, float _smoothness )
	{
		List<Vector3> points;
		List<Vector3> curvedPoints;
		int pointsLength = 0;
		int curvedLength = 0;
		
		_smoothness = Mathf.Clamp(_smoothness,1f,10f);
		
		pointsLength = _listToCurve.Count;
		
		curvedLength = (pointsLength * Mathf.RoundToInt(_smoothness)) - 1;
		curvedPoints = new List<Vector3>(curvedLength);
		
		float t = 0f;
		for ( int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve ++ )
		{
			t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);
			
			points = new List<Vector3>(_listToCurve);
			
			for ( int j = pointsLength - 1; j > 0; j -- )
			{
				for ( int i = 0; i < j; i ++ )
				{
					points[i] = (1 - t) * points[i] + t * points[i + 1];
				}
			}
			
			curvedPoints.Add(points[0]);
		}
		
		return curvedPoints;
	}
	#endregion

	#region Chaikin - smooth function
	public static Vector3[] Chaikin ( Vector3[] _pts ) 
	{
        int l = ((_pts.Length - 2) * 2 + 2);
        Vector3[] newPts = new Vector3[0];

		if ( _pts != null && _pts.Length > 0 )
        {
			newPts = new Vector3[l];
			newPts[0] = _pts[0];
			newPts[newPts.Length - 1] = _pts[_pts.Length - 1];

    		int j = 1;
			for ( int i = 0; i < (_pts.Length - 2); i ++ ) 
    		{
				newPts[j] = _pts[i] + (_pts[i + 1] - _pts[i]) * .75f;
				newPts[j + 1] = _pts[i + 1] + (_pts[i + 2] - _pts[i + 1]) * .25f;
    			j += 2;
    		}
        }

		return newPts;
	}
	#endregion

    //#region Chaikin - smooth function
    //public static Vector3[] ChaikinNoGarbage ( Vector3[] _pts, Vector3[] _ptsSmooth ) 
    //{
    //    //int l = ((_pts.Length - 2) * 2 + 2);

    //    if ( _pts != null && _pts.Length > 0 )
    //    {
    //        _ptsSmooth[0] = _pts[0];
    //        _ptsSmooth[_ptsSmooth.Length - 1] = _pts[_pts.Length - 1];

    //        int j = 1;
    //        for ( int i = 0; i < (_pts.Length - 2); i ++ ) 
    //        {
    //            _ptsSmooth[j] = _pts[i] + (_pts[i + 1] - _pts[i]) * .75f;
    //            _ptsSmooth[j + 1] = _pts[i + 1] + (_pts[i + 2] - _pts[i + 1]) * .25f;
    //            j += 2;
    //        }
    //    }

    //    return _ptsSmooth;
    //}
    //#endregion

    public static Vector3[] ChaikinCurveMulti ( Vector3[] _pts , int _passes )
    {
        Vector3[] newPts = new Vector3[(_pts.Length - 2) * 2 + 2];
        newPts[0] = _pts[0];
        newPts[newPts.Length - 1] = _pts[_pts.Length - 1];

        int j = 1;
        for ( int i = 0; i < _pts.Length - 2; i ++ )
        {
            newPts[j] = _pts[i] + (_pts[i + 1] - _pts[i]) * .75f;
            newPts[j + 1] = _pts[i + 1] + (_pts[i + 2] - _pts[i + 1]) * .25f;
            j += 2;
        }

        _passes --;

        if ( _passes > 0 )
        {
            ChaikinCurveMulti(newPts,_passes);
        }

        return newPts;
    }


    #region IsEven 
    public static bool IsEven ( int _value )
    {
        return ( _value % 2 == 0 );
    }
    #endregion

	#region IsPointInside
	public static bool IsPointInside ( Mesh _mesh, Vector3 _localPoint )
	{
		var verts = _mesh.vertices;
		var tris = _mesh.triangles;
		int triangleCount = tris.Length / 3;

		for ( int i = 0; i < triangleCount; i ++ )
		{
			var V1 = verts[tris[i * 3]];
			var V2 = verts[tris[i * 3 + 1]];
			var V3 = verts[tris[i * 3 + 2]];
			var P = new Plane(V1,V2,V3);
			if ( P.GetSide(_localPoint) )
			{
				return false;
			}
		}

		return true;
	}
	#endregion

    #region AngleInRad 
    public static float AngleInRad ( Vector3 _v1, Vector3 _v2 )
    {
        return Mathf.Atan2(_v2.y - _v1.y,_v2.x - _v1.x);
    }
    #endregion

    #region AngleInDegrees 
    public static float AngleInDegrees ( Vector3 _v1, Vector3 _v2 )
    {
        return AngleInRad(_v1,_v2) * 180f / Mathf.PI;
    }
    #endregion

	#region ResetTransform
	public static void ResetTransform ( Transform _t )
	{
		if ( _t != null )
		{
			_t.localPosition = Vector3.zero;
			_t.localScale = Vector3.one;
			_t.localRotation = Quaternion.identity;
		}
	}
	#endregion

	#region Sine Waves
	public static float SinLP ( float _x )
	{
		if ( _x < -3.14159265f )
		{
			_x += 6.28318531f;
		}
		else if ( _x > 3.14159265f )
		{
			_x -= 6.28318531f;
		}

		if ( _x < 0 )
		{
			return _x * (1.27323954f + .405284735f * _x);
		}
		else
		{
			return _x * (1.27323954f - .405284735f * _x);
		}
	}

	private static float sinH = 0;
	public static float SinHP ( float _x )
	{
		if ( _x < -3.14159265f )
		{
			_x += 6.28318531f;
		}
		else if ( _x >  3.14159265f )
		{
			_x -= 6.28318531f;	
		}

		if ( _x < 0 )
		{
			sinH = _x * (1.27323954f + .405284735f * _x);

			if ( sinH < 0 )
			{
				sinH *= (-.255f * (sinH + 1) + 1);
			}
			else
			{
				sinH *= (.255f * (sinH - 1) + 1);
			}
		}
		else
		{
			sinH = _x * (1.27323954f - .405284735f * _x);

			if ( sinH < 0 )
			{
				sinH *= (-.255f * (sinH + 1) + 1);
			}
			else
			{
				sinH *= (.255f * (sinH - 1) + 1);
			}
		}

		return sinH;
	}
	#endregion

	#region GenerateCone 
	public static GameObject GenerateCone ( Vector3 _pos, Quaternion _rot, Vector3 _scale, float _height, float _bottomRadius, float _topRadius )
	{
		GameObject coneO = new GameObject("proceduralCone");

		MeshFilter filter = coneO.AddComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();

		int nbSides = 32;
		int nbHeightSeg = 1; // Not implemented yet

		int nbVerticesCap = nbSides + 1;

		// bottom + top + sides
		Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * nbHeightSeg * 2 + 2];
		int vert = 0;
		float _2pi = (Mathf.PI * 2f);

		// Bottom cap
		vertices[vert ++] = Vector3.zero;
		while ( vert <= nbSides )
		{
			float rad = (float)vert / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * _bottomRadius,0f,Mathf.Sin(rad) * _bottomRadius);
			vert ++;
		}

		// Top cap
		vertices[vert ++] = new Vector3(0f,_height,0f);
		while ( vert <= nbSides * 2 + 1 )
		{
			float rad = (float)(vert - nbSides - 1)  / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius,_height,Mathf.Sin(rad) * _topRadius);
			vert ++;
		}

		// Sides
		int v = 0;
		while ( vert <= vertices.Length - 4 )
		{
			float rad = (float)v / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * _topRadius,_height,Mathf.Sin(rad) * _topRadius);
			vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * _bottomRadius,0f,Mathf.Sin(rad) * _bottomRadius);
			vert += 2;
			v ++;
		}
		vertices[vert] = vertices[nbSides * 2 + 2];
		vertices[vert + 1] = vertices[nbSides * 2 + 3];

		// bottom + top + sides
		Vector3[] normales = new Vector3[vertices.Length];
		vert = 0;

		// Bottom cap
		while ( vert  <= nbSides )
		{
			normales[vert ++] = Vector3.down;
		}

		// Top cap
		while ( vert <= nbSides * 2 + 1 )
		{
			normales[vert ++] = Vector3.up;
		}

		// Sides
		v = 0;
		while ( vert <= vertices.Length - 4 )
		{			
			float rad = (float)v / nbSides * _2pi;
			float cos = Mathf.Cos(rad);
			float sin = Mathf.Sin(rad);

			normales[vert] = new Vector3(cos,0f,sin);
			normales[vert+1] = normales[vert];

			vert += 2;
			v ++;
		}
		normales[vert] = normales[nbSides * 2 + 2];
		normales[vert + 1] = normales[nbSides * 2 + 3];

		Vector2[] uvs = new Vector2[vertices.Length];

		// Bottom cap
		int u = 0;
		uvs[u ++] = new Vector2(.5f,.5f);
		while ( u <= nbSides )
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f,Mathf.Sin(rad) * .5f + .5f);
			u ++;
		}

		// Top cap
		uvs[u ++] = new Vector2(.5f,.5f);
		while ( u <= nbSides * 2 + 1 )
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			u++;
		}

		// Sides
		int u_sides = 0;
		while ( u <= uvs.Length - 4 )
		{
			float t = (float)u_sides / nbSides;
			uvs[u] = new Vector3(t,1f);
			uvs[u + 1] = new Vector3(t,0f);
			u += 2;
			u_sides ++;
		}
		uvs[u] = new Vector2(1f,1f);
		uvs[u + 1] = new Vector2(1f,0f);

		int nbTriangles = nbSides + nbSides + nbSides * 2;
		int[] triangles = new int[nbTriangles * 3 + 3];

		// Bottom cap
		int tri = 0;
		int i = 0;
		while ( tri < nbSides - 1 )
		{
			triangles[i] = 0;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = tri + 2;
			tri ++;
			i += 3;
		}
		triangles[i] = 0;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = 1;
		tri ++;
		i += 3;

		// Top cap
		//tri++;
		while ( tri < nbSides * 2 )
		{
			triangles[i] = tri + 2;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = nbVerticesCap;
			tri ++;
			i += 3;
		}

		triangles[i] = nbVerticesCap + 1;
		triangles[i + 1] = tri + 1;
		triangles[i + 2] = nbVerticesCap;		
		tri ++;
		i += 3;
		tri ++;

		// Sides
		while ( tri <= nbTriangles )
		{
			triangles[i] = tri + 2;
			triangles[i + 1] = tri + 1;
			triangles[i + 2] = tri + 0;
			tri ++;
			i += 3;

			triangles[i] = tri + 1;
			triangles[i + 1] = tri + 2;
			triangles[i + 2] = tri + 0;
			tri ++;
			i += 3;
		}

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();

		// set position, rotation & scale
		Transform coneTr = coneO.transform;
		coneTr.SetPositionAndRotation(_pos,_rot);
		coneTr.localScale = _scale;

		return coneO;
	}
	#endregion
}
