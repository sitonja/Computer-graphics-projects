using System.Collections.Generic;
using UnityEngine;

public class ImageConverter {

    private float resolution = 0.08f;
    private float scale = 2.0f;
    private float aspect;
    private Texture2D img;

    public ImageConverter(Texture2D img)
    {
        this.img = img;
    }

    public ImageConverter(Texture2D img, float resolution)
    {
        this.img = img;
        this.resolution = resolution;
    }

    public ImageConverter(Texture2D img, float resolution, float scale)
    {
        this.img = img;
        this.resolution = resolution;
        this.scale = scale;
    }

    private Vector3 ToWorldPos(Vector3 p)
    {
        float x = (p.x - 0.5f) * scale;
        float y = (p.y - 0.5f) * scale * aspect;

        return new Vector3(x, y);
    }

    public ICollection<Vector3> CreatePointsfromImage(bool detectEdge, int edgeIncrement = 0)
    {
        IList<Vector3> points = new List<Vector3>();

        aspect = (float)img.height / img.width;

        float fw = img.width;
        float fh = img.height;

        int inc = Mathf.FloorToInt(img.width * resolution);
        int margin = Mathf.Max((inc - 1) / 2, 1);

        for (int y = 0; y < img.height; y += inc)
        {
            for (int x = 0; x < img.width; x += inc)
            {
                // if value non-zero then add a point
                int n = Neighbours(img, x, y, margin);

                if (n > 0)
                {
                    Vector3 uv = new Vector3(x / fw, y / fh);
                    points.Add(uv);
                }
            }
        }
        if (detectEdge)
        {
            // distribute points on the boundary
            for (int y = 0; y < img.height; y+=(1+edgeIncrement))
            {
                for (int x = 0; x < img.width; x+=(1+edgeIncrement))
                {
                    if (EdgeDetect(img, x, y))
                    {
                        Vector3 uv = new Vector3(x / fw, y / fh);
                        points.Add(uv);
                    }
                }
            }
        }
        return points;
    }

    /// <summary>
    /// return true if any pixels in box are non-zero 
    /// </summary>
    private int Neighbours(Texture2D img, int cx, int cy, int margin)
    {
        int xmin = Mathf.Max(0, cx - margin);
        int xmax = Mathf.Min(cx + margin, img.width - 1);
        int ymin = Mathf.Max(0, cy - margin);
        int ymax = Mathf.Min(cy + margin, img.height - 1);

        int count = 0;
        for (int y = ymin; y <= ymax; ++y)
        {
            for (int x = xmin; x <= xmax; ++x)
            {
                Color col = img.GetPixel(x, y);

                if (col.grayscale > 0.0f)
                {
                    ++count;
                }
            }
        }

        return count;
    }

    private bool EdgeDetect(Texture2D img, int x, int y)
    {
        Color colp0 = img.GetPixel(x + 1, y);
        Color coln0 = img.GetPixel(x - 1, y);

        if ((colp0.grayscale > 0.0f) != (coln0.grayscale > 0.0f))
            return true;

        Color col0p = img.GetPixel(x, y + 1);
        Color col0n = img.GetPixel(x, y - 1);

        if ((col0p.grayscale > 0.0f) != (col0n.grayscale > 0.0f))
            return true;

        return false;
    }

    public IList<Triangle> CleanTriangleList(IList<Triangle> triangles)
    {
        IList<int> indices = new List<int>();
        IList<Triangle> result  = new List<Triangle>(triangles);
        for (int i = 0; i < triangles.Count; i++)
        {
            Vector3 p = triangles[i].A;
            Vector3 q = triangles[i].B;
            Vector3 r = triangles[i].C;

            Vector3 c = (p + q + r) / 3.0f;

            Color col = img.GetPixelBilinear(c.x, c.y);

            if (col.grayscale == 0.0f)
                indices.Add(i);
        }

        for (int i = indices.Count - 1; i >= 0; i--)
        {
            result.RemoveAt(indices[i]);
        }
        return result;
    }
}
