# DynamicMachine

Bu kod şu anda basit konsol çağrılarını işleyebiliyor.

- İlk önce korunacak (.exe) veya (.dll) dosyasının hedef yöntemi alınır. (Henüz otomatik değil)
- Yöntem içindeki tüm instruction'lar bir (.txt) dosyasına satır satır kaydedilir.
- DynamicMachine ise dinamik olarak yöntem oluşturur. İçine, önceden kaydedilen txt dosyasından okunan Instructions emitlenir. [Bunun için kesinlikle daha gelişmiş filter ve yöntemler gerekiyor.]
- Yöntem runtime oluşturulup çağrılır.

## Hatırlatma

bin\debug içindeki dnlib kütüphanesini başvurulara eklemeyi unutmayın.
