; ModuleID = 'obj\Debug\130\android\marshal_methods.x86.ll'
source_filename = "obj\Debug\130\android\marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android"


%struct.MonoImage = type opaque

%struct.MonoClass = type opaque

%struct.MarshalMethodsManagedClass = type {
	i32,; uint32_t token
	%struct.MonoClass*; MonoClass* klass
}

%struct.MarshalMethodName = type {
	i64,; uint64_t id
	i8*; char* name
}

%class._JNIEnv = type opaque

%class._jobject = type {
	i8; uint8_t b
}

%class._jclass = type {
	i8; uint8_t b
}

%class._jstring = type {
	i8; uint8_t b
}

%class._jthrowable = type {
	i8; uint8_t b
}

%class._jarray = type {
	i8; uint8_t b
}

%class._jobjectArray = type {
	i8; uint8_t b
}

%class._jbooleanArray = type {
	i8; uint8_t b
}

%class._jbyteArray = type {
	i8; uint8_t b
}

%class._jcharArray = type {
	i8; uint8_t b
}

%class._jshortArray = type {
	i8; uint8_t b
}

%class._jintArray = type {
	i8; uint8_t b
}

%class._jlongArray = type {
	i8; uint8_t b
}

%class._jfloatArray = type {
	i8; uint8_t b
}

%class._jdoubleArray = type {
	i8; uint8_t b
}

; assembly_image_cache
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 4
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [302 x i32] [
	i32 32687329, ; 0: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 79
	i32 34715100, ; 1: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 113
	i32 39109920, ; 2: Newtonsoft.Json.dll => 0x254c520 => 23
	i32 57263871, ; 3: Xamarin.Forms.Core.dll => 0x369c6ff => 106
	i32 95598293, ; 4: Supabase.dll => 0x5b2b6d5 => 25
	i32 101534019, ; 5: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 95
	i32 120558881, ; 6: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 95
	i32 122350210, ; 7: System.Threading.Channels.dll => 0x74aea82 => 42
	i32 134690465, ; 8: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x80736a1 => 121
	i32 162612358, ; 9: MimeMapping => 0x9b14486 => 20
	i32 165246403, ; 10: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 56
	i32 182336117, ; 11: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 97
	i32 209399409, ; 12: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 54
	i32 212497893, ; 13: Xamarin.Forms.Maps.Android => 0xcaa75e5 => 107
	i32 220171995, ; 14: System.Diagnostics.Debug => 0xd1f8edb => 138
	i32 230216969, ; 15: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 73
	i32 230752869, ; 16: Microsoft.CSharp.dll => 0xdc10265 => 15
	i32 231814094, ; 17: System.Globalization => 0xdd133ce => 146
	i32 232815796, ; 18: System.Web.Services => 0xde07cb4 => 132
	i32 261689757, ; 19: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 59
	i32 278686392, ; 20: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 77
	i32 280482487, ; 21: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 71
	i32 318968648, ; 22: Xamarin.AndroidX.Activity.dll => 0x13031348 => 46
	i32 319314094, ; 23: Xamarin.Forms.Maps => 0x130858ae => 108
	i32 321597661, ; 24: System.Numerics => 0x132b30dd => 35
	i32 342366114, ; 25: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 75
	i32 385762202, ; 26: System.Memory.dll => 0x16fe439a => 147
	i32 441335492, ; 27: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 58
	i32 442521989, ; 28: Xamarin.Essentials => 0x1a605985 => 105
	i32 442565967, ; 29: System.Collections => 0x1a61054f => 134
	i32 450948140, ; 30: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 70
	i32 465846621, ; 31: mscorlib => 0x1bc4415d => 22
	i32 469710990, ; 32: System.dll => 0x1bff388e => 33
	i32 476646585, ; 33: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 71
	i32 485463106, ; 34: Microsoft.IdentityModel.Abstractions => 0x1cef9442 => 16
	i32 486930444, ; 35: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 83
	i32 498788369, ; 36: System.ObjectModel => 0x1dbae811 => 145
	i32 520798577, ; 37: FFImageLoading.Platform => 0x1f0ac171 => 11
	i32 526420162, ; 38: System.Transactions.dll => 0x1f6088c2 => 126
	i32 527452488, ; 39: Xamarin.Kotlin.StdLib.Jdk7 => 0x1f704948 => 121
	i32 545304856, ; 40: System.Runtime.Extensions => 0x2080b118 => 141
	i32 548916678, ; 41: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 14
	i32 577335427, ; 42: System.Security.Cryptography.Cng => 0x22697083 => 149
	i32 605376203, ; 43: System.IO.Compression.FileSystem => 0x24154ecb => 130
	i32 610194910, ; 44: System.Reactive.dll => 0x245ed5de => 37
	i32 627609679, ; 45: Xamarin.AndroidX.CustomView => 0x2568904f => 64
	i32 639843206, ; 46: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x26233b86 => 69
	i32 662205335, ; 47: System.Text.Encodings.Web.dll => 0x27787397 => 40
	i32 663517072, ; 48: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 102
	i32 666292255, ; 49: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 51
	i32 690569205, ; 50: System.Xml.Linq.dll => 0x29293ff5 => 44
	i32 691348768, ; 51: Xamarin.KotlinX.Coroutines.Android.dll => 0x29352520 => 123
	i32 700284507, ; 52: Xamarin.Jetbrains.Annotations => 0x29bd7e5b => 118
	i32 720511267, ; 53: Xamarin.Kotlin.StdLib.Jdk8 => 0x2af22123 => 122
	i32 763346851, ; 54: Websocket.Client => 0x2d7fbfa3 => 45
	i32 772621961, ; 55: Supabase.Core.dll => 0x2e0d4689 => 24
	i32 775507847, ; 56: System.IO.Compression => 0x2e394f87 => 129
	i32 809851609, ; 57: System.Drawing.Common.dll => 0x30455ad9 => 128
	i32 843511501, ; 58: Xamarin.AndroidX.Print => 0x3246f6cd => 90
	i32 877678880, ; 59: System.Globalization.dll => 0x34505120 => 146
	i32 920281169, ; 60: Supabase.Functions => 0x36da6051 => 26
	i32 928116545, ; 61: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 113
	i32 955402788, ; 62: Newtonsoft.Json => 0x38f24a24 => 23
	i32 956575887, ; 63: Xamarin.Kotlin.StdLib.Jdk8.dll => 0x3904308f => 122
	i32 967690846, ; 64: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 75
	i32 974778368, ; 65: FormsViewGroup.dll => 0x3a19f000 => 12
	i32 992768348, ; 66: System.Collections.dll => 0x3b2c715c => 134
	i32 1012816738, ; 67: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 94
	i32 1035644815, ; 68: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 50
	i32 1042160112, ; 69: Xamarin.Forms.Platform.dll => 0x3e1e19f0 => 110
	i32 1052210849, ; 70: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 80
	i32 1084122840, ; 71: Xamarin.Kotlin.StdLib => 0x409e66d8 => 120
	i32 1089187994, ; 72: Websocket.Client.dll => 0x40ebb09a => 45
	i32 1098259244, ; 73: System => 0x41761b2c => 33
	i32 1175144683, ; 74: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 100
	i32 1178241025, ; 75: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 87
	i32 1204270330, ; 76: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 51
	i32 1216849306, ; 77: Supabase.Realtime.dll => 0x4887a59a => 29
	i32 1219540809, ; 78: Supabase.Functions.dll => 0x48b0b749 => 26
	i32 1264511973, ; 79: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0x4b5eebe5 => 96
	i32 1267360935, ; 80: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 101
	i32 1275534314, ; 81: Xamarin.KotlinX.Coroutines.Android => 0x4c071bea => 123
	i32 1293217323, ; 82: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 66
	i32 1324164729, ; 83: System.Linq => 0x4eed2679 => 144
	i32 1336984896, ; 84: Supabase.Core => 0x4fb0c540 => 24
	i32 1364015309, ; 85: System.IO => 0x514d38cd => 137
	i32 1365406463, ; 86: System.ServiceModel.Internals.dll => 0x516272ff => 133
	i32 1376866003, ; 87: Xamarin.AndroidX.SavedState => 0x52114ed3 => 94
	i32 1379779777, ; 88: System.Resources.ResourceManager => 0x523dc4c1 => 3
	i32 1391782176, ; 89: ChickenAndPointMobile.Android.dll => 0x52f4e920 => 0
	i32 1395857551, ; 90: Xamarin.AndroidX.Media.dll => 0x5333188f => 84
	i32 1406073936, ; 91: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 60
	i32 1411638395, ; 92: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 38
	i32 1457743152, ; 93: System.Runtime.Extensions.dll => 0x56e36530 => 141
	i32 1460219004, ; 94: Xamarin.Forms.Xaml => 0x57092c7c => 111
	i32 1460893475, ; 95: System.IdentityModel.Tokens.Jwt => 0x57137723 => 34
	i32 1462112819, ; 96: System.IO.Compression.dll => 0x57261233 => 129
	i32 1469204771, ; 97: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 49
	i32 1498168481, ; 98: Microsoft.IdentityModel.JsonWebTokens.dll => 0x594c3ca1 => 17
	i32 1516168485, ; 99: Supabase.Gotrue => 0x5a5ee525 => 27
	i32 1530663695, ; 100: Xamarin.Forms.Maps.dll => 0x5b3c130f => 108
	i32 1530772511, ; 101: FFImageLoading.Forms.Platform => 0x5b3dbc1f => 10
	i32 1543031311, ; 102: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 143
	i32 1550322496, ; 103: System.Reflection.Extensions.dll => 0x5c680b40 => 5
	i32 1582372066, ; 104: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 65
	i32 1592978981, ; 105: System.Runtime.Serialization.dll => 0x5ef2ee25 => 6
	i32 1622152042, ; 106: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 82
	i32 1624863272, ; 107: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 104
	i32 1635184631, ; 108: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x6176eff7 => 69
	i32 1636350590, ; 109: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 63
	i32 1639515021, ; 110: System.Net.Http.dll => 0x61b9038d => 4
	i32 1639986890, ; 111: System.Text.RegularExpressions => 0x61c036ca => 143
	i32 1657153582, ; 112: System.Runtime => 0x62c6282e => 39
	i32 1658241508, ; 113: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 98
	i32 1658251792, ; 114: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 112
	i32 1670060433, ; 115: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 59
	i32 1677501392, ; 116: System.Net.Primitives.dll => 0x63fca3d0 => 139
	i32 1698840827, ; 117: Xamarin.Kotlin.StdLib.Common => 0x654240fb => 119
	i32 1701541528, ; 118: System.Diagnostics.Debug.dll => 0x656b7698 => 138
	i32 1726116996, ; 119: System.Reflection.dll => 0x66e27484 => 135
	i32 1729485958, ; 120: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 55
	i32 1765942094, ; 121: System.Reflection.Extensions => 0x6942234e => 5
	i32 1766324549, ; 122: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 97
	i32 1776026572, ; 123: System.Core.dll => 0x69dc03cc => 32
	i32 1788241197, ; 124: Xamarin.AndroidX.Fragment => 0x6a96652d => 70
	i32 1793089559, ; 125: FFImageLoading.Forms.dll => 0x6ae06017 => 9
	i32 1796167890, ; 126: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 14
	i32 1808609942, ; 127: Xamarin.AndroidX.Loader => 0x6bcd3296 => 82
	i32 1813058853, ; 128: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 120
	i32 1813201214, ; 129: Xamarin.Google.Android.Material => 0x6c13413e => 112
	i32 1818569960, ; 130: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 88
	i32 1840964413, ; 131: FFImageLoading.Forms => 0x6dbae33d => 9
	i32 1867746548, ; 132: Xamarin.Essentials.dll => 0x6f538cf4 => 105
	i32 1878053835, ; 133: Xamarin.Forms.Xaml.dll => 0x6ff0d3cb => 111
	i32 1881862856, ; 134: Xamarin.Forms.Maps.Android.dll => 0x702af2c8 => 107
	i32 1885316902, ; 135: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 52
	i32 1900610850, ; 136: System.Resources.ResourceManager.dll => 0x71490522 => 3
	i32 1904755420, ; 137: System.Runtime.InteropServices.WindowsRuntime.dll => 0x718842dc => 2
	i32 1908813208, ; 138: Xamarin.GooglePlayServices.Basement => 0x71c62d98 => 115
	i32 1919157823, ; 139: Xamarin.AndroidX.MultiDex.dll => 0x7264063f => 85
	i32 1983156543, ; 140: Xamarin.Kotlin.StdLib.Common.dll => 0x7634913f => 119
	i32 1986222447, ; 141: Microsoft.IdentityModel.Tokens.dll => 0x7663596f => 19
	i32 2011961780, ; 142: System.Buffers.dll => 0x77ec19b4 => 31
	i32 2018526534, ; 143: Supabase.Gotrue.dll => 0x78504546 => 27
	i32 2019465201, ; 144: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 80
	i32 2055257422, ; 145: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 76
	i32 2079903147, ; 146: System.Runtime.dll => 0x7bf8cdab => 39
	i32 2090596640, ; 147: System.Numerics.Vectors => 0x7c9bf920 => 36
	i32 2097448633, ; 148: Xamarin.AndroidX.Legacy.Support.Core.UI => 0x7d0486b9 => 72
	i32 2126786730, ; 149: Xamarin.Forms.Platform.Android => 0x7ec430aa => 109
	i32 2128198166, ; 150: Supabase.Realtime => 0x7ed9ba16 => 29
	i32 2129483829, ; 151: Xamarin.GooglePlayServices.Base.dll => 0x7eed5835 => 114
	i32 2138252982, ; 152: Supabase => 0x7f7326b6 => 25
	i32 2193016926, ; 153: System.ObjectModel.dll => 0x82b6c85e => 145
	i32 2201107256, ; 154: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 124
	i32 2201231467, ; 155: System.Net.Http => 0x8334206b => 4
	i32 2217644978, ; 156: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 100
	i32 2244775296, ; 157: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 83
	i32 2256548716, ; 158: Xamarin.AndroidX.MultiDex => 0x8680336c => 85
	i32 2261435625, ; 159: Xamarin.AndroidX.Legacy.Support.V4.dll => 0x86cac4e9 => 74
	i32 2279755925, ; 160: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 92
	i32 2315684594, ; 161: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 47
	i32 2340826669, ; 162: FFImageLoading.dll => 0x8b862e2d => 8
	i32 2353062107, ; 163: System.Net.Primitives => 0x8c40e0db => 139
	i32 2369706906, ; 164: Microsoft.IdentityModel.Logging => 0x8d3edb9a => 18
	i32 2389760966, ; 165: ChickenAndPointMobile.Android => 0x8e70dbc6 => 0
	i32 2403452196, ; 166: Xamarin.AndroidX.Emoji2.dll => 0x8f41c524 => 68
	i32 2409053734, ; 167: Xamarin.AndroidX.Preference.dll => 0x8f973e26 => 89
	i32 2454642406, ; 168: System.Text.Encoding.dll => 0x924edee6 => 142
	i32 2465532216, ; 169: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 58
	i32 2471841756, ; 170: netstandard.dll => 0x93554fdc => 1
	i32 2475788418, ; 171: Java.Interop.dll => 0x93918882 => 13
	i32 2501346920, ; 172: System.Data.DataSetExtensions => 0x95178668 => 127
	i32 2505896520, ; 173: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 79
	i32 2562349572, ; 174: Microsoft.CSharp => 0x98ba5a04 => 15
	i32 2570120770, ; 175: System.Text.Encodings.Web => 0x9930ee42 => 40
	i32 2581819634, ; 176: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 101
	i32 2605712449, ; 177: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 124
	i32 2620871830, ; 178: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 63
	i32 2624644809, ; 179: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 67
	i32 2633051222, ; 180: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 77
	i32 2640290731, ; 181: Microsoft.IdentityModel.Logging.dll => 0x9d5fa3ab => 18
	i32 2693849962, ; 182: System.IO.dll => 0xa090e36a => 137
	i32 2701096212, ; 183: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 98
	i32 2715334215, ; 184: System.Threading.Tasks.dll => 0xa1d8b647 => 136
	i32 2719963679, ; 185: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 149
	i32 2732626843, ; 186: Xamarin.AndroidX.Activity => 0xa2e0939b => 46
	i32 2735172069, ; 187: System.Threading.Channels => 0xa30769e5 => 42
	i32 2737747696, ; 188: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 49
	i32 2766581644, ; 189: Xamarin.Forms.Core => 0xa4e6af8c => 106
	i32 2770495804, ; 190: Xamarin.Jetbrains.Annotations.dll => 0xa522693c => 118
	i32 2778768386, ; 191: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 103
	i32 2779977773, ; 192: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0xa5b3182d => 93
	i32 2810250172, ; 193: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 60
	i32 2819470561, ; 194: System.Xml.dll => 0xa80db4e1 => 43
	i32 2821294376, ; 195: Xamarin.AndroidX.ResourceInspection.Annotation => 0xa8298928 => 93
	i32 2825712786, ; 196: ChickenAndPointMobile => 0xa86cf492 => 7
	i32 2842369275, ; 197: FFImageLoading.Forms.Platform.dll => 0xa96b1cfb => 10
	i32 2847418871, ; 198: Xamarin.GooglePlayServices.Base => 0xa9b829f7 => 114
	i32 2853208004, ; 199: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 103
	i32 2855708567, ; 200: Xamarin.AndroidX.Transition => 0xaa36a797 => 99
	i32 2873222696, ; 201: FFImageLoading => 0xab41e628 => 8
	i32 2901442782, ; 202: System.Reflection => 0xacf080de => 135
	i32 2903344695, ; 203: System.ComponentModel.Composition => 0xad0d8637 => 131
	i32 2905242038, ; 204: mscorlib.dll => 0xad2a79b6 => 22
	i32 2916838712, ; 205: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 104
	i32 2919462931, ; 206: System.Numerics.Vectors.dll => 0xae037813 => 36
	i32 2921128767, ; 207: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 48
	i32 2978675010, ; 208: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 66
	i32 2996846495, ; 209: Xamarin.AndroidX.Lifecycle.Process.dll => 0xb2a03f9f => 78
	i32 3016983068, ; 210: Xamarin.AndroidX.Startup.StartupRuntime => 0xb3d3821c => 96
	i32 3017076677, ; 211: Xamarin.GooglePlayServices.Maps => 0xb3d4efc5 => 116
	i32 3024354802, ; 212: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 73
	i32 3044182254, ; 213: FormsViewGroup => 0xb57288ee => 12
	i32 3057625584, ; 214: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 86
	i32 3058099980, ; 215: Xamarin.GooglePlayServices.Tasks => 0xb646e70c => 117
	i32 3075834255, ; 216: System.Threading.Tasks => 0xb755818f => 136
	i32 3084678329, ; 217: Microsoft.IdentityModel.Tokens => 0xb7dc74b9 => 19
	i32 3099081453, ; 218: MimeMapping.dll => 0xb8b83aed => 20
	i32 3111772706, ; 219: System.Runtime.Serialization => 0xb979e222 => 6
	i32 3124832203, ; 220: System.Threading.Tasks.Extensions => 0xba4127cb => 148
	i32 3138360719, ; 221: Supabase.Postgrest.dll => 0xbb0f958f => 28
	i32 3204380047, ; 222: System.Data.dll => 0xbefef58f => 125
	i32 3211777861, ; 223: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 65
	i32 3220365878, ; 224: System.Threading => 0xbff2e236 => 140
	i32 3230466174, ; 225: Xamarin.GooglePlayServices.Basement.dll => 0xc08d007e => 115
	i32 3247949154, ; 226: Mono.Security => 0xc197c562 => 150
	i32 3258312781, ; 227: Xamarin.AndroidX.CardView => 0xc235e84d => 55
	i32 3265893370, ; 228: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 148
	i32 3267021929, ; 229: Xamarin.AndroidX.AsyncLayoutInflater => 0xc2bacc69 => 53
	i32 3299363146, ; 230: System.Text.Encoding => 0xc4a8494a => 142
	i32 3312457198, ; 231: Microsoft.IdentityModel.JsonWebTokens => 0xc57015ee => 17
	i32 3317135071, ; 232: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 64
	i32 3317144872, ; 233: System.Data => 0xc5b79d28 => 125
	i32 3340431453, ; 234: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 52
	i32 3345895724, ; 235: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xc76e512c => 91
	i32 3346324047, ; 236: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 87
	i32 3353484488, ; 237: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 0xc7e21cc8 => 72
	i32 3358260929, ; 238: System.Text.Json => 0xc82afec1 => 41
	i32 3362522851, ; 239: Xamarin.AndroidX.Core => 0xc86c06e3 => 62
	i32 3366347497, ; 240: Java.Interop => 0xc8a662e9 => 13
	i32 3374999561, ; 241: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 92
	i32 3395150330, ; 242: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 38
	i32 3404865022, ; 243: System.ServiceModel.Internals => 0xcaf21dfe => 133
	i32 3429136800, ; 244: System.Xml => 0xcc6479a0 => 43
	i32 3430777524, ; 245: netstandard => 0xcc7d82b4 => 1
	i32 3441283291, ; 246: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 67
	i32 3476120550, ; 247: Mono.Android => 0xcf3163e6 => 21
	i32 3485117614, ; 248: System.Text.Json.dll => 0xcfbaacae => 41
	i32 3486566296, ; 249: System.Transactions => 0xcfd0c798 => 126
	i32 3493954962, ; 250: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 57
	i32 3501239056, ; 251: Xamarin.AndroidX.AsyncLayoutInflater.dll => 0xd0b0ab10 => 53
	i32 3509114376, ; 252: System.Xml.Linq => 0xd128d608 => 44
	i32 3536029504, ; 253: Xamarin.Forms.Platform.Android.dll => 0xd2c38740 => 109
	i32 3567349600, ; 254: System.ComponentModel.Composition.dll => 0xd4a16f60 => 131
	i32 3607666123, ; 255: Supabase.Postgrest => 0xd7089dcb => 28
	i32 3608519521, ; 256: System.Linq.dll => 0xd715a361 => 144
	i32 3618140916, ; 257: Xamarin.AndroidX.Preference => 0xd7a872f4 => 89
	i32 3627220390, ; 258: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 90
	i32 3632359727, ; 259: Xamarin.Forms.Platform => 0xd881692f => 110
	i32 3633644679, ; 260: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 48
	i32 3641597786, ; 261: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 76
	i32 3672681054, ; 262: Mono.Android.dll => 0xdae8aa5e => 21
	i32 3676310014, ; 263: System.Web.Services.dll => 0xdb2009fe => 132
	i32 3682565725, ; 264: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 54
	i32 3684561358, ; 265: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 57
	i32 3684933406, ; 266: System.Runtime.InteropServices.WindowsRuntime => 0xdba39f1e => 2
	i32 3689375977, ; 267: System.Drawing.Common => 0xdbe768e9 => 128
	i32 3700591436, ; 268: Microsoft.IdentityModel.Abstractions.dll => 0xdc928b4c => 16
	i32 3706696989, ; 269: Xamarin.AndroidX.Core.Core.Ktx.dll => 0xdcefb51d => 61
	i32 3718780102, ; 270: Xamarin.AndroidX.Annotation => 0xdda814c6 => 47
	i32 3724971120, ; 271: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 86
	i32 3731644420, ; 272: System.Reactive => 0xde6c6004 => 37
	i32 3758932259, ; 273: Xamarin.AndroidX.Legacy.Support.V4 => 0xe00cc123 => 74
	i32 3786282454, ; 274: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 56
	i32 3822602673, ; 275: Xamarin.AndroidX.Media => 0xe3d849b1 => 84
	i32 3829621856, ; 276: System.Numerics.dll => 0xe4436460 => 35
	i32 3885922214, ; 277: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 99
	i32 3888767677, ; 278: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0xe7c9e2bd => 91
	i32 3896760992, ; 279: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 62
	i32 3906640997, ; 280: Supabase.Storage.dll => 0xe8da9c65 => 30
	i32 3920810846, ; 281: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 130
	i32 3921031405, ; 282: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 102
	i32 3931092270, ; 283: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 88
	i32 3945713374, ; 284: System.Data.DataSetExtensions.dll => 0xeb2ecede => 127
	i32 3955647286, ; 285: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 50
	i32 3959773229, ; 286: Xamarin.AndroidX.Lifecycle.Process => 0xec05582d => 78
	i32 3970018735, ; 287: Xamarin.GooglePlayServices.Tasks.dll => 0xeca1adaf => 117
	i32 3980364293, ; 288: Supabase.Storage => 0xed3f8a05 => 30
	i32 4025784931, ; 289: System.Memory => 0xeff49a63 => 147
	i32 4073602200, ; 290: System.Threading.dll => 0xf2ce3c98 => 140
	i32 4101593132, ; 291: Xamarin.AndroidX.Emoji2 => 0xf479582c => 68
	i32 4105002889, ; 292: Mono.Security.dll => 0xf4ad5f89 => 150
	i32 4119305463, ; 293: ChickenAndPointMobile.dll => 0xf5879cf7 => 7
	i32 4151237749, ; 294: System.Core => 0xf76edc75 => 32
	i32 4182413190, ; 295: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 81
	i32 4184283386, ; 296: FFImageLoading.Platform.dll => 0xf96718fa => 11
	i32 4256097574, ; 297: Xamarin.AndroidX.Core.Core.Ktx => 0xfdaee526 => 61
	i32 4260525087, ; 298: System.Buffers => 0xfdf2741f => 31
	i32 4263231520, ; 299: System.IdentityModel.Tokens.Jwt.dll => 0xfe1bc020 => 34
	i32 4278134329, ; 300: Xamarin.GooglePlayServices.Maps.dll => 0xfeff2639 => 116
	i32 4292120959 ; 301: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 81
], align 4
@assembly_image_cache_indices = local_unnamed_addr constant [302 x i32] [
	i32 79, i32 113, i32 23, i32 106, i32 25, i32 95, i32 95, i32 42, ; 0..7
	i32 121, i32 20, i32 56, i32 97, i32 54, i32 107, i32 138, i32 73, ; 8..15
	i32 15, i32 146, i32 132, i32 59, i32 77, i32 71, i32 46, i32 108, ; 16..23
	i32 35, i32 75, i32 147, i32 58, i32 105, i32 134, i32 70, i32 22, ; 24..31
	i32 33, i32 71, i32 16, i32 83, i32 145, i32 11, i32 126, i32 121, ; 32..39
	i32 141, i32 14, i32 149, i32 130, i32 37, i32 64, i32 69, i32 40, ; 40..47
	i32 102, i32 51, i32 44, i32 123, i32 118, i32 122, i32 45, i32 24, ; 48..55
	i32 129, i32 128, i32 90, i32 146, i32 26, i32 113, i32 23, i32 122, ; 56..63
	i32 75, i32 12, i32 134, i32 94, i32 50, i32 110, i32 80, i32 120, ; 64..71
	i32 45, i32 33, i32 100, i32 87, i32 51, i32 29, i32 26, i32 96, ; 72..79
	i32 101, i32 123, i32 66, i32 144, i32 24, i32 137, i32 133, i32 94, ; 80..87
	i32 3, i32 0, i32 84, i32 60, i32 38, i32 141, i32 111, i32 34, ; 88..95
	i32 129, i32 49, i32 17, i32 27, i32 108, i32 10, i32 143, i32 5, ; 96..103
	i32 65, i32 6, i32 82, i32 104, i32 69, i32 63, i32 4, i32 143, ; 104..111
	i32 39, i32 98, i32 112, i32 59, i32 139, i32 119, i32 138, i32 135, ; 112..119
	i32 55, i32 5, i32 97, i32 32, i32 70, i32 9, i32 14, i32 82, ; 120..127
	i32 120, i32 112, i32 88, i32 9, i32 105, i32 111, i32 107, i32 52, ; 128..135
	i32 3, i32 2, i32 115, i32 85, i32 119, i32 19, i32 31, i32 27, ; 136..143
	i32 80, i32 76, i32 39, i32 36, i32 72, i32 109, i32 29, i32 114, ; 144..151
	i32 25, i32 145, i32 124, i32 4, i32 100, i32 83, i32 85, i32 74, ; 152..159
	i32 92, i32 47, i32 8, i32 139, i32 18, i32 0, i32 68, i32 89, ; 160..167
	i32 142, i32 58, i32 1, i32 13, i32 127, i32 79, i32 15, i32 40, ; 168..175
	i32 101, i32 124, i32 63, i32 67, i32 77, i32 18, i32 137, i32 98, ; 176..183
	i32 136, i32 149, i32 46, i32 42, i32 49, i32 106, i32 118, i32 103, ; 184..191
	i32 93, i32 60, i32 43, i32 93, i32 7, i32 10, i32 114, i32 103, ; 192..199
	i32 99, i32 8, i32 135, i32 131, i32 22, i32 104, i32 36, i32 48, ; 200..207
	i32 66, i32 78, i32 96, i32 116, i32 73, i32 12, i32 86, i32 117, ; 208..215
	i32 136, i32 19, i32 20, i32 6, i32 148, i32 28, i32 125, i32 65, ; 216..223
	i32 140, i32 115, i32 150, i32 55, i32 148, i32 53, i32 142, i32 17, ; 224..231
	i32 64, i32 125, i32 52, i32 91, i32 87, i32 72, i32 41, i32 62, ; 232..239
	i32 13, i32 92, i32 38, i32 133, i32 43, i32 1, i32 67, i32 21, ; 240..247
	i32 41, i32 126, i32 57, i32 53, i32 44, i32 109, i32 131, i32 28, ; 248..255
	i32 144, i32 89, i32 90, i32 110, i32 48, i32 76, i32 21, i32 132, ; 256..263
	i32 54, i32 57, i32 2, i32 128, i32 16, i32 61, i32 47, i32 86, ; 264..271
	i32 37, i32 74, i32 56, i32 84, i32 35, i32 99, i32 91, i32 62, ; 272..279
	i32 30, i32 130, i32 102, i32 88, i32 127, i32 50, i32 78, i32 117, ; 280..287
	i32 30, i32 147, i32 140, i32 68, i32 150, i32 7, i32 32, i32 81, ; 288..295
	i32 11, i32 61, i32 31, i32 34, i32 116, i32 81 ; 296..301
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 4; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 4

; Function attributes: "frame-pointer"="none" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 4
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 4
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2}
!llvm.ident = !{!3}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"NumRegisterParameters", i32 0}
!3 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
