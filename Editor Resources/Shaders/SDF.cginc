#ifndef SDF_2D
#define SDF_2D

float2 Translate(float2 pos, float2 offset)
{
    return pos - offset;
}

float2 Rotate(float2 pos, float rotation)
{
    const float PI = 3.14159;
    float angle = rotation * PI * 2 * -1;
    
    float sine, cosine;
    sincos(angle, sine, cosine);

    float x = cosine * pos.x + sine * pos.y;
    float y = cosine * pos.y - sine * pos.x;
    
    return float2(x, y);
}

float2 RotateMatrix(float2 pos, float rotation)
{
    const float PI = 3.14159;
    float angle = rotation * PI * 2 * -1;

    float s = sin(angle);
    float c = cos(angle);

    float2x2 rotationMat = float2x2(c, s, -s, c);

    float2 newPos = mul(rotationMat, pos);

    return newPos;
}

float Circle(float2 pos, float radius)
{
    return length(pos) - radius;
}

//rectSize - Full rectangle size. 
float AABoxSDF(float2 pos, float2 rectSize)
{
    float2 dist = abs(pos) - rectSize * 0.5;
    
    float outsideDist = length(max(dist, 0));
    float insideDist  = min(max(dist.x, dist.y), 0);
    
    return outsideDist + insideDist;
}

float AABoxSDFOutside(float2 pos, float2 rectSize)
{
    float2 dist = abs(pos) - rectSize * 0.5;
    
    float outsideDist = length(max(dist, 0));

    return outsideDist;
}

float AABoxSDFRoundedOutside(float2 pos, float2 rectSize, float rounding)
{
    float outsideDist = AABoxSDFOutside(pos, rectSize);
    float roundedDist = outsideDist - rounding;

    return roundedDist;
}

float Union(float shape1, float shape2)
{
    return min(shape1, shape2);
}

float Intersect(float shape1, float shape2)
{
    return max(shape1, shape2);
}

float Subtract(float base, float subtraction)
{
    return Intersect(base, -subtraction);
}


#endif
