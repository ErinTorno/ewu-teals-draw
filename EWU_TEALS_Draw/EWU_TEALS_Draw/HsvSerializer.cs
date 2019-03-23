using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EWU_TEALS_Draw {
    class HsvSerializer {
        private Dictionary<Color, HsvConfig> content;

        public HsvSerializer() : this(new Dictionary<Color, HsvConfig>()) {}

        private HsvSerializer(Dictionary<Color, HsvConfig> defContent) {
            content = defContent;
        }

        public void AddColor(Color color, bool isEnabled, MCvScalar min, MCvScalar max) {
            content[color] = new HsvConfig(isEnabled, min, max);
        }

        public static HsvSerializer FromJson(string json) {
            var newContent = JsonConvert.DeserializeObject<Dictionary<Color, HsvConfig>>(json);
            return new HsvSerializer(newContent);
        }

        public string AsJson() {
            return JsonConvert.SerializeObject(content);
        }

        // Accessors

        public bool IsEnabled(Color color) {
            return content[color].IsEnabled;
        }

        public MCvScalar MinHsv(Color color) {
            return content[color].MinHsv;
        }

        public MCvScalar MaxHsv(Color color) {
            return content[color].MaxHsv;
        }

        private struct HsvConfig {
            public bool IsEnabled;
            public MCvScalar MinHsv;
            public MCvScalar MaxHsv;

            public HsvConfig(bool isEnabled, MCvScalar minHsv, MCvScalar maxHsv) {
                IsEnabled = isEnabled;
                MinHsv = minHsv;
                MaxHsv = maxHsv;
            }
        }
    }
}
