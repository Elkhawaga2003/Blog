using Blog.Services.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Blog.Services.Implemetation
{
    public class ToxicDetector:IToxicDetector
    {
        public bool IsToxic(string content)
        {
            using var session = new InferenceSession(@"D:\New folder\toxic_pipeline.onnx");

            while (true)
            {
            
                var inputTensor = new DenseTensor<string>(new[] { content }, new[] { 1 });

                var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);
                var result = results.First().AsEnumerable<long>().First();

                return result == 1 ? true : false;
            }
        }
    }
}
