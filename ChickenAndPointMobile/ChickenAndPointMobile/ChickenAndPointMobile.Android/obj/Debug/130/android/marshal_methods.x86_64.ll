; ModuleID = 'obj\Debug\130\android\marshal_methods.x86_64.ll'
source_filename = "obj\Debug\130\android\marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android"


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
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 8
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [302 x i64] [
	i64 2646484529726201, ; 0: FFImageLoading.Forms.Platform => 0x966f6b24c42f9 => 10
	i64 24362543149721218, ; 1: Xamarin.AndroidX.DynamicAnimation => 0x568d9a9a43a682 => 67
	i64 120698629574877762, ; 2: Mono.Android => 0x1accec39cafe242 => 21
	i64 210515253464952879, ; 3: Xamarin.AndroidX.Collection.dll => 0x2ebe681f694702f => 56
	i64 232391251801502327, ; 4: Xamarin.AndroidX.SavedState.dll => 0x3399e9cbc897277 => 94
	i64 295915112840604065, ; 5: Xamarin.AndroidX.SlidingPaneLayout => 0x41b4d3a3088a9a1 => 95
	i64 316157742385208084, ; 6: Xamarin.AndroidX.Core.Core.Ktx.dll => 0x46337caa7dc1b14 => 61
	i64 464346026994987652, ; 7: System.Reactive.dll => 0x671b04057e67284 => 37
	i64 634308326490598313, ; 8: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x8cd840fee8b6ba9 => 79
	i64 687654259221141486, ; 9: Xamarin.GooglePlayServices.Base => 0x98b09e7c92917ee => 114
	i64 702024105029695270, ; 10: System.Drawing.Common => 0x9be17343c0e7726 => 128
	i64 720058930071658100, ; 11: Xamarin.AndroidX.Legacy.Support.Core.UI => 0x9fe29c82844de74 => 72
	i64 872800313462103108, ; 12: Xamarin.AndroidX.DrawerLayout => 0xc1ccf42c3c21c44 => 66
	i64 940822596282819491, ; 13: System.Transactions => 0xd0e792aa81923a3 => 126
	i64 996343623809489702, ; 14: Xamarin.Forms.Platform => 0xdd3b93f3b63db26 => 110
	i64 1000557547492888992, ; 15: Mono.Security.dll => 0xde2b1c9cba651a0 => 150
	i64 1120440138749646132, ; 16: Xamarin.Google.Android.Material.dll => 0xf8c9a5eae431534 => 112
	i64 1315114680217950157, ; 17: Xamarin.AndroidX.Arch.Core.Common.dll => 0x124039d5794ad7cd => 51
	i64 1425944114962822056, ; 18: System.Runtime.Serialization.dll => 0x13c9f89e19eaf3a8 => 6
	i64 1476839205573959279, ; 19: System.Net.Primitives.dll => 0x147ec96ece9b1e6f => 139
	i64 1624659445732251991, ; 20: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0x168bf32877da9957 => 49
	i64 1628611045998245443, ; 21: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0x1699fd1e1a00b643 => 81
	i64 1636321030536304333, ; 22: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0x16b5614ec39e16cd => 73
	i64 1731380447121279447, ; 23: Newtonsoft.Json => 0x18071957e9b889d7 => 23
	i64 1743969030606105336, ; 24: System.Memory.dll => 0x1833d297e88f2af8 => 147
	i64 1795316252682057001, ; 25: Xamarin.AndroidX.AppCompat.dll => 0x18ea3e9eac997529 => 50
	i64 1836611346387731153, ; 26: Xamarin.AndroidX.SavedState => 0x197cf449ebe482d1 => 94
	i64 1865037103900624886, ; 27: Microsoft.Bcl.AsyncInterfaces => 0x19e1f15d56eb87f6 => 14
	i64 1875917498431009007, ; 28: Xamarin.AndroidX.Annotation.dll => 0x1a08990699eb70ef => 47
	i64 1948281462830376119, ; 29: ChickenAndPointMobile => 0x1b09afab027ba4b7 => 7
	i64 1981742497975770890, ; 30: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x1b80904d5c241f0a => 80
	i64 1984538867944326539, ; 31: FFImageLoading.Platform.dll => 0x1b8a7f95fac7058b => 11
	i64 2040001226662520565, ; 32: System.Threading.Tasks.Extensions.dll => 0x1c4f8a4ea894a6f5 => 148
	i64 2064708342624596306, ; 33: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x1ca7514c5eecb152 => 121
	i64 2133195048986300728, ; 34: Newtonsoft.Json.dll => 0x1d9aa1984b735138 => 23
	i64 2136356949452311481, ; 35: Xamarin.AndroidX.MultiDex.dll => 0x1da5dd539d8acbb9 => 85
	i64 2152408820173588167, ; 36: Supabase.Functions.dll => 0x1ddee46b01dd46c7 => 26
	i64 2165725771938924357, ; 37: Xamarin.AndroidX.Browser => 0x1e0e341d75540745 => 54
	i64 2262844636196693701, ; 38: Xamarin.AndroidX.DrawerLayout.dll => 0x1f673d352266e6c5 => 66
	i64 2284400282711631002, ; 39: System.Web.Services => 0x1fb3d1f42fd4249a => 132
	i64 2304837677853103545, ; 40: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0x1ffc6da80d5ed5b9 => 93
	i64 2329709569556905518, ; 41: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x2054ca829b447e2e => 76
	i64 2335503487726329082, ; 42: System.Text.Encodings.Web => 0x2069600c4d9d1cfa => 40
	i64 2337758774805907496, ; 43: System.Runtime.CompilerServices.Unsafe => 0x207163383edbc828 => 38
	i64 2470498323731680442, ; 44: Xamarin.AndroidX.CoordinatorLayout => 0x2248f922dc398cba => 60
	i64 2479423007379663237, ; 45: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x2268ae16b2cba985 => 100
	i64 2497223385847772520, ; 46: System.Runtime => 0x22a7eb7046413568 => 39
	i64 2547086958574651984, ; 47: Xamarin.AndroidX.Activity.dll => 0x2359121801df4a50 => 46
	i64 2592350477072141967, ; 48: System.Xml.dll => 0x23f9e10627330e8f => 43
	i64 2612152650457191105, ; 49: Microsoft.IdentityModel.Tokens.dll => 0x24403afeed9892c1 => 19
	i64 2624866290265602282, ; 50: mscorlib.dll => 0x246d65fbde2db8ea => 22
	i64 2694427813909235223, ; 51: Xamarin.AndroidX.Preference.dll => 0x256487d230fe0617 => 89
	i64 2783046991838674048, ; 52: System.Runtime.CompilerServices.Unsafe.dll => 0x269f5e7e6dc37c80 => 38
	i64 2787234703088983483, ; 53: Xamarin.AndroidX.Startup.StartupRuntime => 0x26ae3f31ef429dbb => 96
	i64 2789714023057451704, ; 54: Microsoft.IdentityModel.JsonWebTokens.dll => 0x26b70e1f9943eab8 => 17
	i64 2863324215353042462, ; 55: FFImageLoading.Forms => 0x27bc92340ce4661e => 9
	i64 2926123043691605432, ; 56: Websocket.Client.dll => 0x289bad67ac52adb8 => 45
	i64 2960931600190307745, ; 57: Xamarin.Forms.Core => 0x2917579a49927da1 => 106
	i64 3017704767998173186, ; 58: Xamarin.Google.Android.Material => 0x29e10a7f7d88a002 => 112
	i64 3289520064315143713, ; 59: Xamarin.AndroidX.Lifecycle.Common => 0x2da6b911e3063621 => 75
	i64 3303437397778967116, ; 60: Xamarin.AndroidX.Annotation.Experimental => 0x2dd82acf985b2a4c => 48
	i64 3311221304742556517, ; 61: System.Numerics.Vectors.dll => 0x2df3d23ba9e2b365 => 36
	i64 3344514922410554693, ; 62: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x2e6a1a9a18463545 => 124
	i64 3402534845034375023, ; 63: System.IdentityModel.Tokens.Jwt.dll => 0x2f383b6a0629a76f => 34
	i64 3411255996856937470, ; 64: Xamarin.GooglePlayServices.Basement => 0x2f5737416a942bfe => 115
	i64 3493805808809882663, ; 65: Xamarin.AndroidX.Tracing.Tracing.dll => 0x307c7ddf444f3427 => 98
	i64 3522470458906976663, ; 66: Xamarin.AndroidX.SwipeRefreshLayout => 0x30e2543832f52197 => 97
	i64 3531994851595924923, ; 67: System.Numerics => 0x31042a9aade235bb => 35
	i64 3571415421602489686, ; 68: System.Runtime.dll => 0x319037675df7e556 => 39
	i64 3716579019761409177, ; 69: netstandard.dll => 0x3393f0ed5c8c5c99 => 1
	i64 3727469159507183293, ; 70: Xamarin.AndroidX.RecyclerView => 0x33baa1739ba646bd => 92
	i64 3772598417116884899, ; 71: Xamarin.AndroidX.DynamicAnimation.dll => 0x345af645b473efa3 => 67
	i64 3966267475168208030, ; 72: System.Memory => 0x370b03412596249e => 147
	i64 4084167866418059728, ; 73: Supabase.Postgrest.dll => 0x38ade10920e9d9d0 => 28
	i64 4201423742386704971, ; 74: Xamarin.AndroidX.Core.Core.Ktx => 0x3a4e74a233da124b => 61
	i64 4247996603072512073, ; 75: Xamarin.GooglePlayServices.Tasks => 0x3af3ea6755340049 => 117
	i64 4525561845656915374, ; 76: System.ServiceModel.Internals => 0x3ece06856b710dae => 133
	i64 4636684751163556186, ; 77: Xamarin.AndroidX.VersionedParcelable.dll => 0x4058d0370893015a => 102
	i64 4759461199762736555, ; 78: Xamarin.AndroidX.Lifecycle.Process.dll => 0x420d00be961cc5ab => 78
	i64 4782108999019072045, ; 79: Xamarin.AndroidX.AsyncLayoutInflater.dll => 0x425d76cc43bb0a2d => 53
	i64 4794310189461587505, ; 80: Xamarin.AndroidX.Activity => 0x4288cfb749e4c631 => 46
	i64 4795410492532947900, ; 81: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0x428cb86f8f9b7bbc => 97
	i64 5081566143765835342, ; 82: System.Resources.ResourceManager.dll => 0x4685597c05d06e4e => 3
	i64 5099468265966638712, ; 83: System.Resources.ResourceManager => 0x46c4f35ea8519678 => 3
	i64 5142919913060024034, ; 84: Xamarin.Forms.Platform.Android.dll => 0x475f52699e39bee2 => 109
	i64 5203618020066742981, ; 85: Xamarin.Essentials => 0x4836f704f0e652c5 => 105
	i64 5205316157927637098, ; 86: Xamarin.AndroidX.LocalBroadcastManager => 0x483cff7778e0c06a => 83
	i64 5256995213548036366, ; 87: Xamarin.Forms.Maps.Android.dll => 0x48f4994b4175a10e => 107
	i64 5348796042099802469, ; 88: Xamarin.AndroidX.Media => 0x4a3abda9415fc165 => 84
	i64 5376510917114486089, ; 89: Xamarin.AndroidX.VectorDrawable.Animated => 0x4a9d3431719e5d49 => 100
	i64 5408338804355907810, ; 90: Xamarin.AndroidX.Transition => 0x4b0e477cea9840e2 => 99
	i64 5446034149219586269, ; 91: System.Diagnostics.Debug => 0x4b94333452e150dd => 138
	i64 5451019430259338467, ; 92: Xamarin.AndroidX.ConstraintLayout.dll => 0x4ba5e94a845c2ce3 => 59
	i64 5507995362134886206, ; 93: System.Core.dll => 0x4c705499688c873e => 32
	i64 5692067934154308417, ; 94: Xamarin.AndroidX.ViewPager2.dll => 0x4efe49a0d4a8bb41 => 104
	i64 5748194408492950188, ; 95: Supabase.Storage.dll => 0x4fc5b05bfa1be2ac => 30
	i64 5757522595884336624, ; 96: Xamarin.AndroidX.Concurrent.Futures.dll => 0x4fe6d44bd9f885f0 => 57
	i64 5814345312393086621, ; 97: Xamarin.AndroidX.Preference => 0x50b0b44182a5c69d => 89
	i64 5896680224035167651, ; 98: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x51d5376bfbafdda3 => 77
	i64 6085203216496545422, ; 99: Xamarin.Forms.Platform.dll => 0x5472fc15a9574e8e => 110
	i64 6086316965293125504, ; 100: FormsViewGroup.dll => 0x5476f10882baef80 => 12
	i64 6222399776351216807, ; 101: System.Text.Json.dll => 0x565a67a0ffe264a7 => 41
	i64 6319713645133255417, ; 102: Xamarin.AndroidX.Lifecycle.Runtime => 0x57b42213b45b52f9 => 79
	i64 6401687960814735282, ; 103: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0x58d75d486341cfb2 => 76
	i64 6504860066809920875, ; 104: Xamarin.AndroidX.Browser.dll => 0x5a45e7c43bd43d6b => 54
	i64 6548213210057960872, ; 105: Xamarin.AndroidX.CustomView.dll => 0x5adfed387b066da8 => 64
	i64 6591024623626361694, ; 106: System.Web.Services.dll => 0x5b7805f9751a1b5e => 132
	i64 6659513131007730089, ; 107: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 0x5c6b57e8b6c3e1a9 => 72
	i64 6724398223859243234, ; 108: Supabase.Postgrest => 0x5d51dc8ea565d8e2 => 28
	i64 6876862101832370452, ; 109: System.Xml.Linq => 0x5f6f85a57d108914 => 44
	i64 6894844156784520562, ; 110: System.Numerics.Vectors => 0x5faf683aead1ad72 => 36
	i64 7036436454368433159, ; 111: Xamarin.AndroidX.Legacy.Support.V4.dll => 0x61a671acb33d5407 => 74
	i64 7103753931438454322, ; 112: Xamarin.AndroidX.Interpolator.dll => 0x62959a90372c7632 => 71
	i64 7141281584637745974, ; 113: Xamarin.GooglePlayServices.Maps.dll => 0x631aedc3dd5f1b36 => 116
	i64 7141577505875122296, ; 114: System.Runtime.InteropServices.WindowsRuntime.dll => 0x631bfae7659b5878 => 2
	i64 7270811800166795866, ; 115: System.Linq => 0x64e71ccf51a90a5a => 144
	i64 7338192458477945005, ; 116: System.Reflection => 0x65d67f295d0740ad => 135
	i64 7488575175965059935, ; 117: System.Xml.Linq.dll => 0x67ecc3724534ab5f => 44
	i64 7489048572193775167, ; 118: System.ObjectModel => 0x67ee71ff6b419e3f => 145
	i64 7496222613193209122, ; 119: System.IdentityModel.Tokens.Jwt => 0x6807eec000a1b522 => 34
	i64 7602111570124318452, ; 120: System.Reactive => 0x698020320025a6f4 => 37
	i64 7635363394907363464, ; 121: Xamarin.Forms.Core.dll => 0x69f6428dc4795888 => 106
	i64 7637365915383206639, ; 122: Xamarin.Essentials.dll => 0x69fd5fd5e61792ef => 105
	i64 7654504624184590948, ; 123: System.Net.Http => 0x6a3a4366801b8264 => 4
	i64 7735176074855944702, ; 124: Microsoft.CSharp => 0x6b58dda848e391fe => 15
	i64 7735352534559001595, ; 125: Xamarin.Kotlin.StdLib.dll => 0x6b597e2582ce8bfb => 120
	i64 7820441508502274321, ; 126: System.Data => 0x6c87ca1e14ff8111 => 125
	i64 7836164640616011524, ; 127: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x6cbfa6390d64d704 => 49
	i64 7868980864444657808, ; 128: Supabase.Realtime.dll => 0x6d343c679191f090 => 29
	i64 8044118961405839122, ; 129: System.ComponentModel.Composition => 0x6fa2739369944712 => 131
	i64 8064050204834738623, ; 130: System.Collections.dll => 0x6fe942efa61731bf => 134
	i64 8083354569033831015, ; 131: Xamarin.AndroidX.Lifecycle.Common.dll => 0x702dd82730cad267 => 75
	i64 8103644804370223335, ; 132: System.Data.DataSetExtensions.dll => 0x7075ee03be6d50e7 => 127
	i64 8113615946733131500, ; 133: System.Reflection.Extensions => 0x70995ab73cf916ec => 5
	i64 8167236081217502503, ; 134: Java.Interop.dll => 0x7157d9f1a9b8fd27 => 13
	i64 8185542183669246576, ; 135: System.Collections => 0x7198e33f4794aa70 => 134
	i64 8187640529827139739, ; 136: Xamarin.KotlinX.Coroutines.Android => 0x71a057ae90f0109b => 123
	i64 8290740647658429042, ; 137: System.Runtime.Extensions => 0x730ea0b15c929a72 => 141
	i64 8398329775253868912, ; 138: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x748cdc6f3097d170 => 58
	i64 8400357532724379117, ; 139: Xamarin.AndroidX.Navigation.UI.dll => 0x749410ab44503ded => 88
	i64 8426919725312979251, ; 140: Xamarin.AndroidX.Lifecycle.Process => 0x74f26ed7aa033133 => 78
	i64 8598790081731763592, ; 141: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x77550a055fc61d88 => 69
	i64 8601935802264776013, ; 142: Xamarin.AndroidX.Transition.dll => 0x7760370982b4ed4d => 99
	i64 8626175481042262068, ; 143: Java.Interop => 0x77b654e585b55834 => 13
	i64 8638972117149407195, ; 144: Microsoft.CSharp.dll => 0x77e3cb5e8b31d7db => 15
	i64 8639588376636138208, ; 145: Xamarin.AndroidX.Navigation.Runtime => 0x77e5fbdaa2fda2e0 => 87
	i64 8684531736582871431, ; 146: System.IO.Compression.FileSystem => 0x7885a79a0fa0d987 => 130
	i64 8758604146903086415, ; 147: Supabase.Realtime => 0x798cd011086bf54f => 29
	i64 8853378295825400934, ; 148: Xamarin.Kotlin.StdLib.Common.dll => 0x7add84a720d38466 => 119
	i64 8951477988056063522, ; 149: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0x7c3a09cd9ccf5e22 => 91
	i64 9114191852432800567, ; 150: FFImageLoading.dll => 0x7e7c1d3363043b37 => 8
	i64 9238306115887712111, ; 151: FFImageLoading.Forms.dll => 0x80350e773bce476f => 9
	i64 9312692141327339315, ; 152: Xamarin.AndroidX.ViewPager2 => 0x813d54296a634f33 => 104
	i64 9324707631942237306, ; 153: Xamarin.AndroidX.AppCompat => 0x8168042fd44a7c7a => 50
	i64 9427266486299436557, ; 154: Microsoft.IdentityModel.Logging.dll => 0x82d460ebe6d2a60d => 18
	i64 9584643793929893533, ; 155: System.IO.dll => 0x85037ebfbbd7f69d => 137
	i64 9659729154652888475, ; 156: System.Text.RegularExpressions => 0x860e407c9991dd9b => 143
	i64 9662334977499516867, ; 157: System.Numerics.dll => 0x8617827802b0cfc3 => 35
	i64 9678050649315576968, ; 158: Xamarin.AndroidX.CoordinatorLayout.dll => 0x864f57c9feb18c88 => 60
	i64 9711637524876806384, ; 159: Xamarin.AndroidX.Media.dll => 0x86c6aadfd9a2c8f0 => 84
	i64 9808709177481450983, ; 160: Mono.Android.dll => 0x881f890734e555e7 => 21
	i64 9825649861376906464, ; 161: Xamarin.AndroidX.Concurrent.Futures => 0x885bb87d8abc94e0 => 57
	i64 9834056768316610435, ; 162: System.Transactions.dll => 0x8879968718899783 => 126
	i64 9875200773399460291, ; 163: Xamarin.GooglePlayServices.Base.dll => 0x890bc2c8482339c3 => 114
	i64 9907349773706910547, ; 164: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x897dfa20b758db53 => 69
	i64 9998632235833408227, ; 165: Mono.Security => 0x8ac2470b209ebae3 => 150
	i64 10038780035334861115, ; 166: System.Net.Http.dll => 0x8b50e941206af13b => 4
	i64 10226222362177979215, ; 167: Xamarin.Kotlin.StdLib.Jdk7 => 0x8dead70ebbc6434f => 121
	i64 10229024438826829339, ; 168: Xamarin.AndroidX.CustomView => 0x8df4cb880b10061b => 64
	i64 10321854143672141184, ; 169: Xamarin.Jetbrains.Annotations.dll => 0x8f3e97a7f8f8c580 => 118
	i64 10347464889647514442, ; 170: Supabase.Gotrue => 0x8f99947e7144434a => 27
	i64 10360651442923773544, ; 171: System.Text.Encoding => 0x8fc86d98211c1e68 => 142
	i64 10376576884623852283, ; 172: Xamarin.AndroidX.Tracing.Tracing => 0x900101b2f888c2fb => 98
	i64 10406448008575299332, ; 173: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x906b2153fcb3af04 => 124
	i64 10430153318873392755, ; 174: Xamarin.AndroidX.Core => 0x90bf592ea44f6673 => 62
	i64 10447083246144586668, ; 175: Microsoft.Bcl.AsyncInterfaces.dll => 0x90fb7edc816203ac => 14
	i64 10566960649245365243, ; 176: System.Globalization.dll => 0x92a562b96dcd13fb => 146
	i64 10714184849103829812, ; 177: System.Runtime.Extensions.dll => 0x94b06e5aa4b4bb34 => 141
	i64 10775409704848971057, ; 178: Xamarin.Forms.Maps => 0x9589f20936d1d531 => 108
	i64 10785150219063592792, ; 179: System.Net.Primitives => 0x95ac8cfb68830758 => 139
	i64 10847732767863316357, ; 180: Xamarin.AndroidX.Arch.Core.Common => 0x968ae37a86db9f85 => 51
	i64 11023048688141570732, ; 181: System.Core => 0x98f9bc61168392ac => 32
	i64 11037814507248023548, ; 182: System.Xml => 0x992e31d0412bf7fc => 43
	i64 11162124722117608902, ; 183: Xamarin.AndroidX.ViewPager => 0x9ae7d54b986d05c6 => 103
	i64 11340910727871153756, ; 184: Xamarin.AndroidX.CursorAdapter => 0x9d630238642d465c => 63
	i64 11392833485892708388, ; 185: Xamarin.AndroidX.Print.dll => 0x9e1b79b18fcf6824 => 90
	i64 11444370155346000636, ; 186: Xamarin.Forms.Maps.Android => 0x9ed292057b7afafc => 107
	i64 11517440453979132662, ; 187: Microsoft.IdentityModel.Abstractions.dll => 0x9fd62b122523d2f6 => 16
	i64 11529969570048099689, ; 188: Xamarin.AndroidX.ViewPager.dll => 0xa002ae3c4dc7c569 => 103
	i64 11578238080964724296, ; 189: Xamarin.AndroidX.Legacy.Support.V4 => 0xa0ae2a30c4cd8648 => 74
	i64 11580057168383206117, ; 190: Xamarin.AndroidX.Annotation => 0xa0b4a0a4103262e5 => 47
	i64 11591352189662810718, ; 191: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0xa0dcc167234c525e => 96
	i64 11597940890313164233, ; 192: netstandard => 0xa0f429ca8d1805c9 => 1
	i64 11672361001936329215, ; 193: Xamarin.AndroidX.Interpolator => 0xa1fc8e7d0a8999ff => 71
	i64 11743665907891708234, ; 194: System.Threading.Tasks => 0xa2f9e1ec30c0214a => 136
	i64 11868377108928577036, ; 195: MimeMapping.dll => 0xa4b4f2196610be0c => 20
	i64 12102847907131387746, ; 196: System.Buffers => 0xa7f5f40c43256f62 => 31
	i64 12123043025855404482, ; 197: System.Reflection.Extensions.dll => 0xa83db366c0e359c2 => 5
	i64 12137774235383566651, ; 198: Xamarin.AndroidX.VectorDrawable => 0xa872095bbfed113b => 101
	i64 12145679461940342714, ; 199: System.Text.Json => 0xa88e1f1ebcb62fba => 41
	i64 12439275739440478309, ; 200: Microsoft.IdentityModel.JsonWebTokens => 0xaca12f61007bf865 => 17
	i64 12451044538927396471, ; 201: Xamarin.AndroidX.Fragment.dll => 0xaccaff0a2955b677 => 70
	i64 12466513435562512481, ; 202: Xamarin.AndroidX.Loader.dll => 0xad01f3eb52569061 => 82
	i64 12487638416075308985, ; 203: Xamarin.AndroidX.DocumentFile.dll => 0xad4d00fa21b0bfb9 => 65
	i64 12538491095302438457, ; 204: Xamarin.AndroidX.CardView.dll => 0xae01ab382ae67e39 => 55
	i64 12550732019250633519, ; 205: System.IO.Compression => 0xae2d28465e8e1b2f => 129
	i64 12642522128825709279, ; 206: ChickenAndPointMobile.Android => 0xaf7342e4a9abf2df => 0
	i64 12700543734426720211, ; 207: Xamarin.AndroidX.Collection => 0xb041653c70d157d3 => 56
	i64 12708238894395270091, ; 208: System.IO => 0xb05cbbf17d3ba3cb => 137
	i64 12808066478489537992, ; 209: Websocket.Client => 0xb1bf649a25f50dc8 => 45
	i64 12828192437253469131, ; 210: Xamarin.Kotlin.StdLib.Jdk8.dll => 0xb206e50e14d873cb => 122
	i64 12888876061296924636, ; 211: Supabase.Core.dll => 0xb2de7c7d53a397dc => 24
	i64 12963446364377008305, ; 212: System.Drawing.Common.dll => 0xb3e769c8fd8548b1 => 128
	i64 13103606566951232584, ; 213: ChickenAndPointMobile.dll => 0xb5d95cbf2b9aa448 => 7
	i64 13129914918964716986, ; 214: Xamarin.AndroidX.Emoji2.dll => 0xb636d40db3fe65ba => 68
	i64 13310112861600168646, ; 215: Supabase.Storage => 0xb8b70520ac093ac6 => 30
	i64 13370592475155966277, ; 216: System.Runtime.Serialization => 0xb98de304062ea945 => 6
	i64 13401370062847626945, ; 217: Xamarin.AndroidX.VectorDrawable.dll => 0xb9fb3b1193964ec1 => 101
	i64 13404347523447273790, ; 218: Xamarin.AndroidX.ConstraintLayout.Core => 0xba05cf0da4f6393e => 58
	i64 13454009404024712428, ; 219: Xamarin.Google.Guava.ListenableFuture => 0xbab63e4543a86cec => 113
	i64 13465488254036897740, ; 220: Xamarin.Kotlin.StdLib => 0xbadf06394d106fcc => 120
	i64 13491513212026656886, ; 221: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0xbb3b7bc905569876 => 52
	i64 13572454107664307259, ; 222: Xamarin.AndroidX.RecyclerView.dll => 0xbc5b0b19d99f543b => 92
	i64 13647894001087880694, ; 223: System.Data.dll => 0xbd670f48cb071df6 => 125
	i64 13828521679616088467, ; 224: Xamarin.Kotlin.StdLib.Common => 0xbfe8c733724e1993 => 119
	i64 13959074834287824816, ; 225: Xamarin.AndroidX.Fragment => 0xc1b8989a7ad20fb0 => 70
	i64 13967638549803255703, ; 226: Xamarin.Forms.Platform.Android => 0xc1d70541e0134797 => 109
	i64 14124974489674258913, ; 227: Xamarin.AndroidX.CardView => 0xc405fd76067d19e1 => 55
	i64 14125464355221830302, ; 228: System.Threading.dll => 0xc407bafdbc707a9e => 140
	i64 14172845254133543601, ; 229: Xamarin.AndroidX.MultiDex => 0xc4b00faaed35f2b1 => 85
	i64 14212104595480609394, ; 230: System.Security.Cryptography.Cng.dll => 0xc53b89d4a4518272 => 149
	i64 14261073672896646636, ; 231: Xamarin.AndroidX.Print => 0xc5e982f274ae0dec => 90
	i64 14327695147300244862, ; 232: System.Reflection.dll => 0xc6d632d338eb4d7e => 135
	i64 14486659737292545672, ; 233: Xamarin.AndroidX.Lifecycle.LiveData => 0xc90af44707469e88 => 77
	i64 14495724990987328804, ; 234: Xamarin.AndroidX.ResourceInspection.Annotation => 0xc92b2913e18d5d24 => 93
	i64 14551742072151931844, ; 235: System.Text.Encodings.Web.dll => 0xc9f22c50f1b8fbc4 => 40
	i64 14625816794512409936, ; 236: Supabase.Gotrue.dll => 0xcaf956e23adac550 => 27
	i64 14644440854989303794, ; 237: Xamarin.AndroidX.LocalBroadcastManager.dll => 0xcb3b815e37daeff2 => 83
	i64 14744453227118192070, ; 238: MimeMapping => 0xcc9ed21731bde5c6 => 20
	i64 14792063746108907174, ; 239: Xamarin.Google.Guava.ListenableFuture.dll => 0xcd47f79af9c15ea6 => 113
	i64 14852515768018889994, ; 240: Xamarin.AndroidX.CursorAdapter.dll => 0xce1ebc6625a76d0a => 63
	i64 14987728460634540364, ; 241: System.IO.Compression.dll => 0xcfff1ba06622494c => 129
	i64 14988210264188246988, ; 242: Xamarin.AndroidX.DocumentFile => 0xd000d1d307cddbcc => 65
	i64 15076659072870671916, ; 243: System.ObjectModel.dll => 0xd13b0d8c1620662c => 145
	i64 15133485256822086103, ; 244: System.Linq.dll => 0xd204f0a9127dd9d7 => 144
	i64 15138356091203993725, ; 245: Microsoft.IdentityModel.Abstractions => 0xd2163ea89395c07d => 16
	i64 15150743910298169673, ; 246: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xd2424150783c3149 => 91
	i64 15154054061132759083, ; 247: Supabase => 0xd24e03e104e2402b => 25
	i64 15279429628684179188, ; 248: Xamarin.KotlinX.Coroutines.Android.dll => 0xd40b704b1c4c96f4 => 123
	i64 15370334346939861994, ; 249: Xamarin.AndroidX.Core.dll => 0xd54e65a72c560bea => 62
	i64 15398511348637731642, ; 250: FFImageLoading.Forms.Platform.dll => 0xd5b2807c9d5f8b3a => 10
	i64 15498556404192614632, ; 251: ChickenAndPointMobile.Android.dll => 0xd715eeef4b4198e8 => 0
	i64 15526743539506359484, ; 252: System.Text.Encoding.dll => 0xd77a12fc26de2cbc => 142
	i64 15582737692548360875, ; 253: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xd841015ed86f6aab => 81
	i64 15609085926864131306, ; 254: System.dll => 0xd89e9cf3334914ea => 33
	i64 15777549416145007739, ; 255: Xamarin.AndroidX.SlidingPaneLayout.dll => 0xdaf51d99d77eb47b => 95
	i64 15810740023422282496, ; 256: Xamarin.Forms.Xaml => 0xdb6b08484c22eb00 => 111
	i64 15817206913877585035, ; 257: System.Threading.Tasks.dll => 0xdb8201e29086ac8b => 136
	i64 15847085070278954535, ; 258: System.Threading.Channels.dll => 0xdbec27e8f35f8e27 => 42
	i64 15930129725311349754, ; 259: Xamarin.GooglePlayServices.Tasks.dll => 0xdd1330956f12f3fa => 117
	i64 15937190497610202713, ; 260: System.Security.Cryptography.Cng => 0xdd2c465197c97e59 => 149
	i64 15963349826457351533, ; 261: System.Threading.Tasks.Extensions => 0xdd893616f748b56d => 148
	i64 16154507427712707110, ; 262: System => 0xe03056ea4e39aa26 => 33
	i64 16423015068819898779, ; 263: Xamarin.Kotlin.StdLib.Jdk8 => 0xe3ea453135e5c19b => 122
	i64 16565028646146589191, ; 264: System.ComponentModel.Composition.dll => 0xe5e2cdc9d3bcc207 => 131
	i64 16621146507174665210, ; 265: Xamarin.AndroidX.ConstraintLayout => 0xe6aa2caf87dedbfa => 59
	i64 16677317093839702854, ; 266: Xamarin.AndroidX.Navigation.UI => 0xe771bb8960dd8b46 => 88
	i64 16822611501064131242, ; 267: System.Data.DataSetExtensions => 0xe975ec07bb5412aa => 127
	i64 16833383113903931215, ; 268: mscorlib => 0xe99c30c1484d7f4f => 22
	i64 16866861824412579935, ; 269: System.Runtime.InteropServices.WindowsRuntime => 0xea132176ffb5785f => 2
	i64 16890310621557459193, ; 270: System.Text.RegularExpressions.dll => 0xea66700587f088f9 => 143
	i64 17024911836938395553, ; 271: Xamarin.AndroidX.Annotation.Experimental.dll => 0xec44a31d250e5fa1 => 48
	i64 17031351772568316411, ; 272: Xamarin.AndroidX.Navigation.Common.dll => 0xec5b843380a769fb => 86
	i64 17037200463775726619, ; 273: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xec704b8e0a78fc1b => 73
	i64 17118171214553292978, ; 274: System.Threading.Channels => 0xed8ff6060fc420b2 => 42
	i64 17137864900836977098, ; 275: Microsoft.IdentityModel.Tokens => 0xedd5ed53b705e9ca => 19
	i64 17544493274320527064, ; 276: Xamarin.AndroidX.AsyncLayoutInflater => 0xf37a8fada41aded8 => 53
	i64 17576078694130054946, ; 277: Supabase.dll => 0xf3eac67343eef722 => 25
	i64 17627500474728259406, ; 278: System.Globalization => 0xf4a176498a351f4e => 146
	i64 17643123953373031521, ; 279: FFImageLoading => 0xf4d8f7c220fc2c61 => 8
	i64 17685921127322830888, ; 280: System.Diagnostics.Debug.dll => 0xf571038fafa74828 => 138
	i64 17704177640604968747, ; 281: Xamarin.AndroidX.Loader => 0xf5b1dfc36cac272b => 82
	i64 17710060891934109755, ; 282: Xamarin.AndroidX.Lifecycle.ViewModel => 0xf5c6c68c9e45303b => 80
	i64 17790600151040787804, ; 283: Microsoft.IdentityModel.Logging => 0xf6e4e89427cc055c => 18
	i64 17816041456001989629, ; 284: Xamarin.Forms.Maps.dll => 0xf73f4b4f90a1bbfd => 108
	i64 17838668724098252521, ; 285: System.Buffers.dll => 0xf78faeb0f5bf3ee9 => 31
	i64 17882897186074144999, ; 286: FormsViewGroup => 0xf82cd03e3ac830e7 => 12
	i64 17891337867145587222, ; 287: Xamarin.Jetbrains.Annotations => 0xf84accff6fb52a16 => 118
	i64 17892495832318972303, ; 288: Xamarin.Forms.Xaml.dll => 0xf84eea293687918f => 111
	i64 17928294245072900555, ; 289: System.IO.Compression.FileSystem.dll => 0xf8ce18a0b24011cb => 130
	i64 17947624217716767869, ; 290: FFImageLoading.Platform => 0xf912c522ab34bc7d => 11
	i64 17969331831154222830, ; 291: Xamarin.GooglePlayServices.Maps => 0xf95fe418471126ee => 116
	i64 17986907704309214542, ; 292: Xamarin.GooglePlayServices.Basement.dll => 0xf99e554223166d4e => 115
	i64 18025913125965088385, ; 293: System.Threading => 0xfa28e87b91334681 => 140
	i64 18099689198537119569, ; 294: Supabase.Functions => 0xfb2f036e07c79751 => 26
	i64 18116111925905154859, ; 295: Xamarin.AndroidX.Arch.Core.Runtime => 0xfb695bd036cb632b => 52
	i64 18121036031235206392, ; 296: Xamarin.AndroidX.Navigation.Common => 0xfb7ada42d3d42cf8 => 86
	i64 18129453464017766560, ; 297: System.ServiceModel.Internals.dll => 0xfb98c1df1ec108a0 => 133
	i64 18226465428055663763, ; 298: Supabase.Core => 0xfcf169bd26322493 => 24
	i64 18260797123374478311, ; 299: Xamarin.AndroidX.Emoji2 => 0xfd6b623bde35f3e7 => 68
	i64 18305135509493619199, ; 300: Xamarin.AndroidX.Navigation.Runtime.dll => 0xfe08e7c2d8c199ff => 87
	i64 18380184030268848184 ; 301: Xamarin.AndroidX.VersionedParcelable => 0xff1387fe3e7b7838 => 102
], align 16
@assembly_image_cache_indices = local_unnamed_addr constant [302 x i32] [
	i32 10, i32 67, i32 21, i32 56, i32 94, i32 95, i32 61, i32 37, ; 0..7
	i32 79, i32 114, i32 128, i32 72, i32 66, i32 126, i32 110, i32 150, ; 8..15
	i32 112, i32 51, i32 6, i32 139, i32 49, i32 81, i32 73, i32 23, ; 16..23
	i32 147, i32 50, i32 94, i32 14, i32 47, i32 7, i32 80, i32 11, ; 24..31
	i32 148, i32 121, i32 23, i32 85, i32 26, i32 54, i32 66, i32 132, ; 32..39
	i32 93, i32 76, i32 40, i32 38, i32 60, i32 100, i32 39, i32 46, ; 40..47
	i32 43, i32 19, i32 22, i32 89, i32 38, i32 96, i32 17, i32 9, ; 48..55
	i32 45, i32 106, i32 112, i32 75, i32 48, i32 36, i32 124, i32 34, ; 56..63
	i32 115, i32 98, i32 97, i32 35, i32 39, i32 1, i32 92, i32 67, ; 64..71
	i32 147, i32 28, i32 61, i32 117, i32 133, i32 102, i32 78, i32 53, ; 72..79
	i32 46, i32 97, i32 3, i32 3, i32 109, i32 105, i32 83, i32 107, ; 80..87
	i32 84, i32 100, i32 99, i32 138, i32 59, i32 32, i32 104, i32 30, ; 88..95
	i32 57, i32 89, i32 77, i32 110, i32 12, i32 41, i32 79, i32 76, ; 96..103
	i32 54, i32 64, i32 132, i32 72, i32 28, i32 44, i32 36, i32 74, ; 104..111
	i32 71, i32 116, i32 2, i32 144, i32 135, i32 44, i32 145, i32 34, ; 112..119
	i32 37, i32 106, i32 105, i32 4, i32 15, i32 120, i32 125, i32 49, ; 120..127
	i32 29, i32 131, i32 134, i32 75, i32 127, i32 5, i32 13, i32 134, ; 128..135
	i32 123, i32 141, i32 58, i32 88, i32 78, i32 69, i32 99, i32 13, ; 136..143
	i32 15, i32 87, i32 130, i32 29, i32 119, i32 91, i32 8, i32 9, ; 144..151
	i32 104, i32 50, i32 18, i32 137, i32 143, i32 35, i32 60, i32 84, ; 152..159
	i32 21, i32 57, i32 126, i32 114, i32 69, i32 150, i32 4, i32 121, ; 160..167
	i32 64, i32 118, i32 27, i32 142, i32 98, i32 124, i32 62, i32 14, ; 168..175
	i32 146, i32 141, i32 108, i32 139, i32 51, i32 32, i32 43, i32 103, ; 176..183
	i32 63, i32 90, i32 107, i32 16, i32 103, i32 74, i32 47, i32 96, ; 184..191
	i32 1, i32 71, i32 136, i32 20, i32 31, i32 5, i32 101, i32 41, ; 192..199
	i32 17, i32 70, i32 82, i32 65, i32 55, i32 129, i32 0, i32 56, ; 200..207
	i32 137, i32 45, i32 122, i32 24, i32 128, i32 7, i32 68, i32 30, ; 208..215
	i32 6, i32 101, i32 58, i32 113, i32 120, i32 52, i32 92, i32 125, ; 216..223
	i32 119, i32 70, i32 109, i32 55, i32 140, i32 85, i32 149, i32 90, ; 224..231
	i32 135, i32 77, i32 93, i32 40, i32 27, i32 83, i32 20, i32 113, ; 232..239
	i32 63, i32 129, i32 65, i32 145, i32 144, i32 16, i32 91, i32 25, ; 240..247
	i32 123, i32 62, i32 10, i32 0, i32 142, i32 81, i32 33, i32 95, ; 248..255
	i32 111, i32 136, i32 42, i32 117, i32 149, i32 148, i32 33, i32 122, ; 256..263
	i32 131, i32 59, i32 88, i32 127, i32 22, i32 2, i32 143, i32 48, ; 264..271
	i32 86, i32 73, i32 42, i32 19, i32 53, i32 25, i32 146, i32 8, ; 272..279
	i32 138, i32 82, i32 80, i32 18, i32 108, i32 31, i32 12, i32 118, ; 280..287
	i32 111, i32 130, i32 11, i32 116, i32 115, i32 140, i32 26, i32 52, ; 288..295
	i32 86, i32 133, i32 24, i32 68, i32 87, i32 102 ; 296..301
], align 16

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 8; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 8

; Function attributes: "frame-pointer"="none" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 8
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 8
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 16; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="none" "target-cpu"="x86-64" "target-features"="+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="none" "target-cpu"="x86-64" "target-features"="+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1}
!llvm.ident = !{!2}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
