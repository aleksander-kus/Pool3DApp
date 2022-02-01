using System.Drawing;
using System.Numerics;

namespace InfrastructureLayer.Services
{
    public class IlluminationService
    {
        public Color ComputeColor(Vector3 point, Vector3 normalVector)//, IlluminationParameters parameters)
        {
            //// the base color of point
            //var I_O = parameters.ColoringMode == ColoringMode.SolidColor ? parameters.SceneColor.From255() : GetColorFromTexture(point, parameters).From255();
            //// light color
            //var I_L = parameters.LightColor.From255();
            //// normal versor gotten from ball
            //var N_ball = Vector3.Normalize(point - new Vector3(parameters.Radius, parameters.Radius, 0));
            //// normal versor from normalMap
            //var color = parameters.NormalMap.GetPixel((int)point.X, (int)point.Y).From255();
            //var N_normalMap = Multiply(Vector3.Normalize(2 * color - Vector3.One), N_ball);
            //var N = Vector3.Normalize(N_ball * parameters.K + N_normalMap * (1 - parameters.K));
            //// additional versors
            //var V = new Vector3(0, 0, 1);
            //var sourceLocation = new Vector3(parameters.LightSourcePosition.X, parameters.LightSourcePosition.Y, parameters.Radius + parameters.Z);
            //var L = Vector3.Normalize(sourceLocation - point);
            //var R = 2 * Vector3.Dot(N, L) * N - L;

            //var actualColor1 = I_L * I_O * parameters.Kd * CosineBetweenVectors(N, L);
            //var actualColor2 = I_L * I_O * parameters.Ks * (float)Math.Pow(CosineBetweenVectors(V, R), parameters.M);
            //if (parameters.LightMode == LightMode.Normal)
            //    return (actualColor1 + actualColor2).To255();
            //(Vector3 position, Color color) reflector1 = (new Vector3(0, parameters.CanvasY, parameters.Radius + parameters.H), Color.Red);
            //(Vector3 position, Color color) reflector2 = (new Vector3(parameters.CanvasX, parameters.CanvasY, parameters.Radius + parameters.H), Color.Green);
            //(Vector3 position, Color color) reflector3 = (new Vector3(parameters.CanvasX / 2, 0, parameters.Radius + parameters.H), Color.Blue);
            //var ret = CalculateColorFromLight(reflector1.position, reflector1.color, point, N, I_O, parameters) +
            //    CalculateColorFromLight(reflector2.position, reflector2.color, point, N, I_O, parameters) + CalculateColorFromLight(reflector3.position, reflector3.color, point, N, I_O, parameters)
            //    + actualColor1 + actualColor2;
            return normalVector.To255();
        }
    }
}
