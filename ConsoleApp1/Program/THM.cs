using System.IO;

namespace ConsoleApp1
{
    public partial class THM : CBase
    {
        public enum ETType
        {
            ttImage = 0,
            ttCubeMap,
            ttBumpMap,
            ttNormalMap,
            ttTerrain,
            tt1,
            tt2,
            tt3,
            tt4,
            tt5,
            tt6,
            tt7,
            tt8,
            tt9,
            ttForceU32 = -1
        };
        public enum ETFormat
        {
            tfDXT1 = 0,
            tfADXT1,
            tfDXT3,
            tfDXT5,
            tf4444,
            tf1555,
            tf565,
            tfRGB,
            tfRGBA,
            tfNVHS,
            tfNVHU,
            tfA8,
            tfL8,
            tfA8L8,
            tfForceU32 = -1
        };
        public enum ETBumpMode
        {
            tbmResereved = 0,
            tbmNone,
            tbmUse,
            tbmUseParallax,
            tbmForceU32 = -1
        };
        public enum ETMaterial
        {
            tmOrenNayar_Blin = 0,
            tmBlin_Phong,
            tmPhong_Metal,
            tmMetal_OrenNayar,
            tmForceU32 = -1
        };
        public enum ETextureFlags
        {
            flGenerateMipMaps = (1 << 0),
            flBinaryAlpha = (1 << 1),
            flAlphaBorder = (1 << 4),
            flColorBorder = (1 << 5),
            flFadeToColor = (1 << 6),
            flFadeToAlpha = (1 << 7),
            flDitherColor = (1 << 8),
            flDitherEachMIPLevel = (1 << 9),

            flDiffuseDetail = (1 << 23),
            flImplicitLighted = (1 << 24),
            flHasAlpha = (1 << 25),
            flBumpDetail = (1 << 26),

            flForceU32 = -1
        };
        public enum EMIPFilters
        {
            kMIPFilterAdvanced = 5,

            kMIPFilterPoint = 2,
            kMIPFilterBox = 0,
            kMIPFilterTriangle = 3,
            kMIPFilterQuadratic = 4,
            kMIPFilterCubic = 1,

            kMIPFilterCatrom = 6,
            kMIPFilterMitchell = 7,

            kMIPFilterGaussian = 8,
            kMIPFilterSinc = 9,
            kMIPFilterBessel = 10,

            kMIPFilterHanning = 11,
            kMIPFilterHamming = 12,
            kMIPFilterBlackman = 13,
            kMIPFilterKaiser = 14,
        };
        public ETFormat fmt = ETFormat.tfForceU32;
        public ETType type = ETType.ttForceU32;
        public ETMaterial material = ETMaterial.tmForceU32;
        public ETBumpMode bump_mode = ETBumpMode.tbmForceU32;
        public EMIPFilters mip_filter = EMIPFilters.kMIPFilterBox;

        public uint border_color = 0, fade_color = 0, fade_amount = 0, width = 0, height = 0;
        public string detail_name = "", bump_name = "", ext_normal_map_name = "";
        public float material_weight = 0, bump_virtual_height = 0;
        public Flags32 m_flags = new Flags32();
        public System.Byte fade_delay = 0;
        public bool repaired = false;

        int THM_CHUNK_VERSION = 0x0810;
        int THM_CHUNK_TEXTUREPARAM = 0x0812;
        int THM_CHUNK_TYPE = 0x0813;
        int THM_CHUNK_TEXTURE_TYPE = 0x0814;
        int THM_CHUNK_DETAIL_EXT = 0x0815;
        int THM_CHUNK_MATERIAL = 0x0816;
        int THM_CHUNK_BUMP = 0x0817;
        int THM_CHUNK_EXT_NORMALMAP = 0x0818;
        int THM_CHUNK_FADE_DELAY = 0x0819;
        int THM_CHUNK_THM_EDITOR_FLAG = 0x0820;

        private Form1 form;

        public THM(Form1 form1) { form = form1; }

        public void ResetValues()
        {
            repaired = false;

            border_color = 0;
            fade_color = 0;
            fade_amount = 0;
            width = 0;
            height = 0;
            material_weight = 0;
            bump_virtual_height = 0;
            fade_delay = 0;

            detail_name = "";
            bump_name = "";
            ext_normal_map_name = "";

            m_flags = new Flags32();
        }

        // В ТЧ и ЗП tp.fmt и tp.type перепутаны. СДК 0.4 и 0.7 их сохраняют всегда правильно (Как в зп).
        // Для тч использование в рендере tp.fmt вместо tp.type, вероятнее всего, является опечаткой, и как следствие,
        // неправильная загрузка THM файла. Вот такие вот приколы от ГСК.
        public void soc_cop_repair()
        {
            form.Values_Update();

            ETFormat fmt_old = fmt;
            ETType type_old = type;

            fmt = (ETFormat)(uint)type_old;
            type = (ETType)(uint)fmt_old;

            repaired = !repaired;

            form.Form_Update();
        }

        public void OnTypeChange()
        {
            switch (type)
            {
                case ETType.ttImage:
                case ETType.ttCubeMap:
                    break;
                case ETType.ttBumpMap:
                    m_flags.Set((uint)ETextureFlags.flGenerateMipMaps, false);
                    break;
                case ETType.ttNormalMap:
                    m_flags.Set((uint)(ETextureFlags.flImplicitLighted | ETextureFlags.flBinaryAlpha | 
                        ETextureFlags.flAlphaBorder | ETextureFlags.flColorBorder | ETextureFlags.flFadeToColor |
                        ETextureFlags.flFadeToAlpha | ETextureFlags.flDitherColor | ETextureFlags.flDitherEachMIPLevel |
                        ETextureFlags.flBumpDetail), false);
                    m_flags.Set((uint)ETextureFlags.flGenerateMipMaps, true);
                    mip_filter = EMIPFilters.kMIPFilterKaiser;
                    fmt = ETFormat.tfRGBA;
                    break;
                case ETType.ttTerrain:
                    m_flags.Set((uint)(ETextureFlags.flGenerateMipMaps | ETextureFlags.flBinaryAlpha |
                        ETextureFlags.flAlphaBorder | ETextureFlags.flColorBorder | ETextureFlags.flFadeToColor |
                        ETextureFlags.flFadeToAlpha | ETextureFlags.flDitherColor | ETextureFlags.flDitherEachMIPLevel |
                        ETextureFlags.flBumpDetail), false);
                    m_flags.Set((uint)ETextureFlags.flImplicitLighted, true);
                    fmt = ETFormat.tfDXT1;
                    break;
            }
            form.Form_Update();
        }

        public void Load(string filename)
        {
            ResetValues();
            using (IReader reader = new IReader(new BinaryReader(File.Open(filename, FileMode.Open))))
            {
                form.need_update_values = false;
                R_ASSERT(reader.find_chunk(THM_CHUNK_TEXTUREPARAM) != 0, "Can't open chunk: THM_CHUNK_TEXTUREPARAM");
                fmt = (ETFormat)reader.r_u32();
                m_flags.Set(reader.r_u32(), true);
                border_color = reader.r_u32();
                fade_color = reader.r_u32();
                fade_amount = reader.r_u32();
                mip_filter = (EMIPFilters)reader.r_u32();
                width = reader.r_u32();
                height = reader.r_u32();

                if (reader.find_chunk(THM_CHUNK_TEXTURE_TYPE) != 0)
                {
                    type = (ETType)reader.r_u32();
                }
                if (reader.find_chunk(THM_CHUNK_DETAIL_EXT) != 0)
                {
                    detail_name = reader.r_stringZ();
                }
                if (reader.find_chunk(THM_CHUNK_MATERIAL) != 0)
                {
                    material = (ETMaterial)reader.r_u32();
                    material_weight = reader.r_float();
                }
                if (reader.find_chunk(THM_CHUNK_BUMP) != 0)
                {
                    bump_virtual_height = reader.r_float();
                    bump_mode = (ETBumpMode)reader.r_u32();
                    if (bump_mode < ETBumpMode.tbmNone)
                    {
                        bump_mode = ETBumpMode.tbmNone; //.. временно (до полного убирания Autogen)
                    }
                    bump_name = reader.r_stringZ();
                }
                if (reader.find_chunk(THM_CHUNK_EXT_NORMALMAP) != 0)
                {
                    ext_normal_map_name = reader.r_stringZ();
                }
                if(reader.find_chunk(THM_CHUNK_FADE_DELAY) != 0)
                {
                    fade_delay = reader.r_u8();
                }
                if(reader.find_chunk(THM_CHUNK_THM_EDITOR_FLAG) != 0)
                {
                    repaired = reader.r_bool();
                    if(repaired) // Для нормальной загрузки
                    {
                        soc_cop_repair();
                    }
                }
            }
            form.Form_Update();
            form.need_update_values = true;
        }
        public void Save(string filename)
        {
            form.Values_Update();
            using (IWriter writer = new IWriter(new BinaryWriter(File.Create(filename))))
            {
                writer.open_chunk(THM_CHUNK_VERSION);
                writer.w_u16((ushort)0x0012);
                writer.close_chunk();

                writer.open_chunk(THM_CHUNK_TYPE);
                writer.w_u32(1);
                writer.close_chunk();

                writer.open_chunk(THM_CHUNK_TEXTUREPARAM);
                writer.w_u32((uint)fmt);
                writer.w_u32(m_flags.Get());
                writer.w_u32(border_color);
                writer.w_u32(fade_color);
                writer.w_u32(fade_amount);
                writer.w_u32((uint)mip_filter);
                writer.w_u32(width);
                writer.w_u32(height);
                writer.close_chunk();

                if (type != ETType.ttForceU32)
                {
                    writer.open_chunk(THM_CHUNK_TEXTURE_TYPE);
                    writer.w_u32((uint)type);
                    writer.close_chunk();
                }

                if (detail_name != "")
                {
                    writer.open_chunk(THM_CHUNK_DETAIL_EXT);
                    writer.w_stringZ(detail_name);
                    writer.close_chunk();
                }

                if (material != ETMaterial.tmForceU32 || material_weight != 0)
                {
                    writer.open_chunk(THM_CHUNK_MATERIAL);
                    writer.w_u32((uint)material);
                    writer.w_float(material_weight);
                    writer.close_chunk();
                }

                if (bump_virtual_height != 0 || (bump_mode != ETBumpMode.tbmNone && bump_mode != ETBumpMode.tbmForceU32) || bump_name != "")
                {
                    writer.open_chunk(THM_CHUNK_BUMP);
                    writer.w_float(bump_virtual_height);
                    writer.w_u32((uint)bump_mode);
                    writer.w_stringZ(bump_name);
                    writer.close_chunk();
                }

                if (ext_normal_map_name != "")
                {
                    writer.open_chunk(THM_CHUNK_EXT_NORMALMAP);
                    writer.w_stringZ(ext_normal_map_name);
                    writer.close_chunk();
                }

                if (fade_delay != 0)
                {
                    writer.open_chunk(THM_CHUNK_FADE_DELAY);
                    writer.w_u8(fade_delay);
                    writer.close_chunk();
                }

                writer.open_chunk(THM_CHUNK_THM_EDITOR_FLAG);
                writer.w_bool(repaired);
                writer.close_chunk();
            }
        }
    }
}