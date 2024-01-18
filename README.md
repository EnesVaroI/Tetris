Kodun çalıştırılabilmesi için öncelikle sistemde en düşük C# v10 ve .NET v6.0 yüklü
olmalıdır. IDE seçimi, C# projelerine uyumu bakımından Visual Studio olabilir.

Diğer gereksinimler arasında;
● MonoGame.Framework.DesktopGL v3.8.1.303
● MonoGame.Content.Builder.Task v3.8.1.303
● Newtonsoft.Json v13.0.3
● System.Data.SQLite v1.0.118
yer alır. Newtonsoft.Json ve SQLite kütüphaneleri Visual Studio üzerinde NuGet
package olarak indirilebilir. MonoGame framework’üne ait kütüphaneler ise resmi
website’den erişilebilir.

Buraya kadar olan kısım, projenin C# tarafını çalıştırmak için yeterlidir. Fakat bulanık
mantık ve test modülleri Python üzerinden yazıldığı için en az Python v3.11.7’ninde
sistemde yüklü olması gerekir.

Python kütüphanesi olarak ise scikit-fuzzy ve numpy paketleri gereklidir. Bunlar ise;
“pip install numpy scikit-fuzzy” komutu çalıştırılarak yüklenebilir.

Sonuç bölümünde bahsedilecek olan, bulanık mantık test modülü çalıştırılmak
istenirse eğer “python fuzzy-accuracy-calculator.py” komutu çalıştırılabilir.

Dipnot: Test modülünün çalışması için terminal üzerinde, Python dosyasının
bulunduğu klasör açık olmalıdır. Ve Tetris Database.json dosyası ile aynı klasör
içinde olmalıdır.

MonoGame resmi websitesi: https://monogame.net/articles/getting_started/1_setting_up_your_development_environment_windows/
