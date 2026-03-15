# Universal-Audio-Ambient-Manager
Universal Audio & Ambient Manager
Bu modül, oyun dünyasındaki atmosferik müzikleri, rastgele korku seslerini ve evrensel ses efektlerini (SFX) tek bir merkezden, çakışma (audio clipping) yaşatmadan yöneten Singleton tabanlı bir sistemdir.

Özellikler:

Kanal Bazlı Ses Yönetimi (Channel Routing): Awake metodu çalıştığında, VoiceManager kendi altına Ambient, SFX, Earthquake gibi alt (child) AudioSource objeleri oluşturur. Bu sayede bir kapı açılma sesi (SFX), fenerin uzun cızırtı sesini (Flashlight Source) kesintiye uğratmaz.

Evrensel Dinamik SFX (UnityEvent): Kasa, dolap, kapı gibi etkileşimli objelere ait ses klipleri bu manager içinde hardcoded olarak tutulmaz. Bunun yerine PlaySFX(AudioClip) fonksiyonu dışarı açıktır. Etkileşimli objeler kendi ses kliplerini Inspector üzerinden bu fonksiyona göndererek çaldırır. "Open/Closed" prensibine tamamen uyar.

Performanslı Random Horror Events: Oyuncuyu tetikte tutan rastgele fısıltı ve gıcırtı sesleri ağır Update döngüleriyle değil, kendi ritminde uyuyan Coroutine'ler ile hesaplanır.

Kurulum:

Sahnede boş bir objeye VoiceManager scriptini atayın.

Ambient, Random Horror ve Footstep ses dizilerini (Array) doldurun.

Kasa, Kapı veya Fener objelerinizdeki UnityEvent'ler aracılığıyla VoiceManager.Instance.PlaySFX metodunu çağırıp, o objeye ait ses klibini parametre olarak gönderin.
