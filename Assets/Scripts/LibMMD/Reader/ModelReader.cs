using System.IO;
using LibMMD.Model;
using LibMMD.Reader.PMX;

namespace LibMMD.Reader {
    public abstract class ModelReader {
        private MmdModel ReadModel(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("File " + path + " does not exist.");
            }

            using (FileStream stream = File.OpenRead(path)) {
                using (var reader = new BinaryReader(stream)) {
                    return ReadModel(reader);
                }
            }
        }

        protected abstract MmdModel ReadModel(BinaryReader reader);
        
        /// <summary>
        /// Load a model from a file.
        /// </summary>
        /// <param name="path">The path to the model file.</param>
        /// <returns>The loaded model object.</returns>
        /// <note>Only .pmd and .pmx are supported.</note>
        public static MmdModel LoadModel(string path) {
            string extension = Path.GetExtension(path);
            return extension switch {
                ".pmd" => throw new System.NotImplementedException(),
                ".pmx" => new PmxReader().ReadModel(path),
                _ => throw new ModelFormatException("File " + path + " is not a valid model file. Only .pmd and .pmx are supported.")
            };
        }
    }
}