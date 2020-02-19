using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MonteCarlo {

    public class MonteCarloController : MonoBehaviour {

        public SpriteRenderer theRenderer, effectRenderer;
        public TextMeshProUGUI hitRate, samples;
        public TextAsset encodedImage;

        private Texture2D theTexture;
        private Color missColor = new Color(1, 1, 1, 1);
        private double hits = 0, misses = 0;

        public Color sampleColor;
        public float sampleFadeSpeed = 1;
        private SampleEffect theSampleEffect;

        private int width, height;
        private bool[] bitmapImage;

        // Use this for initialization
        void Start() {
            theTexture = theRenderer.sprite.texture;
            theSampleEffect = new SampleEffect(effectRenderer, Color.red, sampleFadeSpeed);
            width = theTexture.width;
            height = theTexture.height;
            decodeImage();
        }



        // Update is called once per frame
        void Update() {
            theSampleEffect.tick(Time.deltaTime);

            if (hits + misses == 0)
                setHitRateText(0);
            else
                setHitRateText(hits/ ( hits + misses));
            
            setSamplesText();
        }


        public bool samplePixel(int x, int y) {
            MainThread.fireEventAtMainThread(() => {theSampleEffect.displaySample(x, y);});
            bool hit = bitmapImage[y * width + x];//theTexture.GetPixel(x, y) != missColor;

            if (hit)
                hits++;
            else
                misses++;

            return hit;
        }

        private void setHitRateText(double percentage) {hitRate.text = (percentage*100) + "%";}
        private void setSamplesText() { samples.text = (hits + misses).ToString(); }
        public bool isInRange(int x, int y) { return x >= 0 && x < width && y >= 0 && y < height; }


        public void restartGame() {
            hits = 0;
            misses = 0;
            theSampleEffect = new SampleEffect(effectRenderer, Color.red, sampleFadeSpeed);
        }


        #region Encode/Decode Image
        private void decodeImage() {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            mStream.Write(encodedImage.bytes, 0, encodedImage.bytes.Length);
            mStream.Position = 0;

            bitmapImage = (binFormatter.Deserialize(mStream) as List<bool>).ToArray();
        }

        /*
        private IEnumerator sampleImage() {
            string result = "";
            List<bool> temp = new List<bool>();
            yield return new WaitForEndOfFrame();
            for(int y = 0; y < height; y++) {
                yield return new WaitForEndOfFrame();
                for(int x = 0; x < width; x++)
                    temp.Add(samplePixel(x, y));
            }
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();
            binFormatter.Serialize(mStream, temp);

            //This gives you the byte array.
            File.WriteAllBytes("EncodedTwitterImage.dat", mStream.ToArray());
        }
        */
        #endregion
    }


    public class SampleEffect {

        public SpriteRenderer theRenderer;
        private Texture2D theTexture;
        private float fadeSpeed;

        private Color effectColor;
        private float r, g, b;
        private Dictionary<string, VisualSample> currentSamples = new Dictionary<string, VisualSample>();

        #region Init
        public SampleEffect(SpriteRenderer renderer, Color effectColor, float fadeSpeed) {
            this.theRenderer = renderer;
            this.fadeSpeed = fadeSpeed;
            this.effectColor = effectColor;
            this.r = effectColor.r; this.g = effectColor.g; this.b = effectColor.b;
            initTexture();
        }

        private void initTexture() {
            theRenderer.sprite = Resources.Load<Sprite>("TwitterIconInstance");
            theTexture = theRenderer.sprite.texture;
            
            int gridSize = 4096;
            int stepSize = 64;
            Color[] grid = new Color[gridSize];
            for (int i = 0; i < gridSize; i++)
                grid[i] = new Color(0, 0, 0, 0);

            for(int y = 0; y < theTexture.height/ stepSize; y++)
                for(int x = 0; x < theTexture.width / stepSize; x++)
                theTexture.SetPixels(x* stepSize, y* stepSize, stepSize, stepSize, grid);

            theTexture.Apply();
        }
        #endregion


        #region Updates
        public void tick(float deltaTime) {theTexture.Apply();}
        public void displaySample(int x, int y) {
            string key = hashCoord(x, y);
            if (currentSamples.ContainsKey(key))
                adjustAlpha(currentSamples[key], 1);
            else {
                currentSamples.Add(key, new VisualSample() { x = x, y = y, alpha = 1 });
                theTexture.SetPixel(x, y, Color.red);
            }
        }
        #endregion


        #region Utils
        private void relativeAdjustAlpha(VisualSample sample, float newAlpha) { sample.alpha += newAlpha; }
        private void adjustAlpha(VisualSample sample, float newAlpha) {sample.alpha = newAlpha;}
        private string hashCoord(int x, int y) { return x + "." + y; }
        private Color getSampleColor(VisualSample sample) {return new Color(r, g, b, sample.alpha);}
        #endregion
    }

    public struct VisualSample {
        public float alpha;
        public int x, y;
    }
}