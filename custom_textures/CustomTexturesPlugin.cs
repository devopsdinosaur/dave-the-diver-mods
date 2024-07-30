using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

[BepInPlugin("devopsdinosaur.davethediver.custom_textures", "Custom Textures", "0.0.1")]
public class CustomTexturesPlugin : BasePlugin {

	const string GAME_ASSEMBLY_NAME = "Assembly-CSharp";

	private Harmony m_harmony = new Harmony("devopsdinosaur.davethediver.custom_textures");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	private static ConfigEntry<string> m_subdir;

	public override void Load() {
		logger = base.Log;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
			m_subdir = this.Config.Bind<string>("General", "Texture Subfolder", "custom_textures", "Subfolder under this plugin's parent folder (i.e. <game>/BepInEx/plugins) which in turn contains a set of mod-specific texture directories.");
			CustomTextureManager.Instance.initialize(GAME_ASSEMBLY_NAME);
			CustomTextureManager.Instance.get_texture_paths();
			this.m_harmony.PatchAll();
			logger.LogInfo("devopsdinosaur.davethediver.custom_textures v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void debug_log(string text) {
		logger.LogInfo(text);
	}

	class CustomTextureManager {

		class CustomTexture {
			public string m_name;
			public string m_path;
			private Texture2D m_texture;
			public Texture2D Texture {
				get {
					return this.get_texture();
				}
			}
			private bool m_load_failed;

			public CustomTexture(string name, string path) {
				this.m_name = name;
				this.m_path = path;
				this.m_texture = null;
				this.m_load_failed = false;
			}

			private Texture2D get_texture() {
				try {
					if (this.m_load_failed) {
						return null;
					}
					if (this.m_texture != null) {
						return this.m_texture;
					}
					// debug_log($"CustomTexture.get_texture - loading '{this.m_name}' from file '{this.m_path}'.");
					this.m_texture = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_UNorm, new TextureCreationFlags());
					this.m_texture.LoadImage(File.ReadAllBytes(this.m_path));
					this.m_texture.filterMode = FilterMode.Point;
					this.m_texture.wrapMode = TextureWrapMode.Clamp;
					this.m_texture.wrapModeU = TextureWrapMode.Clamp;
					this.m_texture.wrapModeV = TextureWrapMode.Clamp;
					this.m_texture.wrapModeW = TextureWrapMode.Clamp;
					this.m_load_failed = false;
					return this.m_texture;
				} catch (Exception e) {
					logger.LogError("** CustomTexture.get_texture ERROR - " + e);
					logger.LogError("** NOTE: Texture load failed.  This texture will no longer be considered for replacement during this run or until a manual reload is triggered.");
				}
				this.m_load_failed = true;
				return null;
			}
		}

		private static CustomTextureManager m_instance = null;
		public static CustomTextureManager Instance {
			get {
				if (m_instance == null) {
					m_instance = new CustomTextureManager();
				}
				return m_instance;
			}
		}
		private string m_main_assembly_name;
		private Dictionary<string, CustomTexture> m_textures = new Dictionary<string, CustomTexture>();
		private Dictionary<string, Sprite> m_sprites = new Dictionary<string, Sprite>();

		public void initialize(string main_assembly_name) {
			this.m_main_assembly_name = main_assembly_name;
			SceneManager.sceneLoaded += new Action<Scene, LoadSceneMode>(this.on_scene_loaded);
		}

		public void get_texture_paths() {
			try {
				string root_dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), m_subdir.Value);
				logger.LogInfo((object) $"Assembling custom texture information from directory, '{root_dir}'.");
				if (!Directory.Exists(root_dir)) {
					logger.LogInfo("Directory does not exist; creating.");
					Directory.CreateDirectory(root_dir);
				}
				this.m_textures.Clear();
				int total_counter = 0;
				foreach (string sub_dir in Directory.GetDirectories(root_dir)) {
					string mod_dir = Path.Combine(root_dir, sub_dir);
					int local_counter = 0;
					foreach (string file in Directory.GetFiles(mod_dir, "*.png", SearchOption.AllDirectories)) {
						string key = Path.GetFileNameWithoutExtension(file);
						string full_path = Path.Combine(mod_dir, file);
						if (m_textures.ContainsKey(key)) {
							logger.LogWarning((object) $"* file '{this.m_textures[key]}' replaces '{full_path}' in texture dictionary; the first file will not be used.");
						}
						this.m_textures[key] = new CustomTexture(key, full_path);
						local_counter++;
					}
					logger.LogInfo((object) $"--> {mod_dir}: {local_counter} textures.");
					total_counter += local_counter;
				}
				logger.LogInfo((object) $"Found {total_counter} total textures.");
			} catch (Exception e) {
				logger.LogError("** get_texture_paths ERROR - " + e);
			}
		}

		private void on_scene_loaded(Scene scene, LoadSceneMode mode) {
			try {
				if (!m_enabled.Value) {
					return;
				}
				Dictionary<Type, List<int>> checked_hashes = new Dictionary<Type, List<int>>();
				foreach (Component component in Resources.FindObjectsOfTypeAll<Component>()) {
					if (component is Renderer renderer) {

					} else if (component is MonoBehaviour) {
						this.replace_members_in_instance<Texture2D>(component, checked_hashes);
						this.replace_members_in_instance<Sprite>(component, checked_hashes);
					}
				}
			} catch (Exception e) {
				logger.LogError("** on_scene_loaded ERROR - " + e);
			}
		}

		private void replace_members_in_instance<T>(object obj, Dictionary<Type, List<int>> checked_hashes) where T : UnityEngine.Object {
			try {
				if (!checked_hashes.ContainsKey(typeof(T))) {
					checked_hashes[typeof(T)] = new List<int>();
				}
				if (obj == null || checked_hashes[typeof(T)].Contains(obj.GetHashCode())) {
					return;
				}
				checked_hashes[typeof(T)].Add(obj.GetHashCode());
				foreach (FieldInfo field in AccessTools.GetDeclaredFields(obj.GetType())) {
					try {
						object val = field.GetValue(obj);
						if (field.FieldType == typeof(T)) {
							// debug_log($"== {obj.GetType().Name}.{field.Name} ({field.FieldType})");
							field.SetValue(obj, this.get_replacement<T>((T) val));
						} else if (field.FieldType == typeof(T[])) {
							// debug_log($"== {obj.GetType().Name}.{field.Name} ({field.FieldType})");
							List<T> new_items = new List<T>();
							foreach (T item in (T[]) val) {
								new_items.Add((T) this.get_replacement<T>(item));
							}
							field.SetValue(obj, new_items.ToArray());
						} else if (field.FieldType.IsGenericType) {
							if (field.FieldType.Name.StartsWith("List") && field.FieldType.GenericTypeArguments.Length == 1 && field.FieldType.GenericTypeArguments[0] == typeof(T)) {
								// debug_log($"== {obj.GetType().Name}.{field.Name} ({field.FieldType})");
								ReflectionUtils.enumerate_list_entries(val, delegate (ReflectionUtils.EnumerateListEntriesCallbackParams info) {
									info.Value = this.get_replacement<T>((T) info.Value);
									// debug_log($"[{info.Index}] = {info.Value} ({(info.Value != null ? info.Value.GetHashCode().ToString() : "---")})");
									return true;
								});
							} else if (field.FieldType.Name.StartsWith("Dictionary") && field.FieldType.GenericTypeArguments.Length == 2 && field.FieldType.GenericTypeArguments[1] == typeof(T)) {
								// debug_log($"== {obj.GetType().Name}.{field.Name} ({field.FieldType})");
								ReflectionUtils.enumerate_dict_entries(val, delegate (ReflectionUtils.EnumerateDictEntriesCallbackParams info) {
									info.Value = this.get_replacement<T>((T) info.Value);
									// debug_log($"[{info.Key}] = {info.Value} ({(info.Value != null ? info.Value.GetHashCode().ToString() : "---")})");
									return true;
								});
							}
						} else if (val != null && field.FieldType.Assembly.GetName().Name == this.m_main_assembly_name && !(val is Enum) && !(val is MonoBehaviour)) {
							// debug_log($"== {obj.GetType().Name}.{field.Name} ({field.FieldType})");
							this.replace_members_in_instance<T>(val, checked_hashes);
						}
					} catch (Exception e) {
						logger.LogWarning($"** replace_members_in_instance WARNING - unable to process {obj.GetType().Name}.{field.Name} ({field.FieldType}).  Exception: " + e);
					}
				}
			} catch (Exception e) {
				logger.LogError("** replace_members_in_instance ERROR - " + e);
			}
		}

		private string get_sprite_key(string sprite_name, string texture_name) {
			return sprite_name + "_" + texture_name;
		}

		private object get_replacement<T>(T original) where T : UnityEngine.Object {
			try {
				if (original == null) {
					// debug_log($"get_replacement(original == null); nothing to do.");
					return original;
				}
				if (original is Sprite sprite) {
					return this.get_replacement_Sprite(sprite);
				}
				if (original is Texture2D texture) {
					return this.get_replacement_Texture2D(texture);
				}
			} catch (Exception e) {
				logger.LogError("** get_replacement ERROR - " + e);
			}
			return original;
		}

		private object get_replacement_Texture2D(Texture2D texture) {
			string texture_name = texture?.name;
			if (string.IsNullOrEmpty(texture_name)) {
				// debug_log($"get_replacement_texture(texture_name == null); nothing to do.");
				return texture;
			}
			// debug_log($"get_replacement_texture(texture.name: {texture_name})");
			if (!this.m_textures.ContainsKey(texture_name) || this.m_textures[texture_name].Texture == null) {
				// debug_log($"texture not in custom texture dict; nothing to replace.");
				return texture;
			}
			return this.m_textures[texture_name].Texture;
		}

		private object get_replacement_Sprite(Sprite sprite) {
			string texture_name = sprite.texture?.name;
			if (string.IsNullOrEmpty(texture_name)) {
				// debug_log($"get_replacement_sprite(texture_name == null); nothing to do.");
				return sprite;
			}
			// debug_log($"get_replacement_sprite(sprite.name: {sprite.name}, sprite.texture.name: {texture_name})");
			string key = get_sprite_key(sprite.name, texture_name);
			if (this.m_sprites.TryGetValue(key, out Sprite replaced_sprite)) {
				// debug_log($"returning cached '{key}' sprite.");
				return replaced_sprite;
			}
			if (!this.m_textures.ContainsKey(texture_name) || this.m_textures[texture_name].Texture == null) {
				// debug_log($"texture not in custom texture dict; nothing to replace.");
				return sprite;
			}
			Sprite new_sprite = Sprite.Create(
				this.m_textures[texture_name].Texture,
				sprite.rect,
				new Vector2(
					sprite.pivot.x / sprite.rect.width,
					sprite.pivot.y / sprite.rect.height
				),
				sprite.pixelsPerUnit,
				0,
				SpriteMeshType.FullRect,
				sprite.border,
				true
			);
			new_sprite.name = sprite.name;
			// debug_log($"adding replacement sprite to dict at key '{key}'.");
			return (this.m_sprites[key] = new_sprite);
		}
	
	}

}