# DynamicMachine

Bu kod şu anda basit konsol çağrılarını işleyebiliyor.

- Programın amacı runtime işlenen mekanizma ile orjinal kodu korumaktır.
- İlk önce korunacak (.exe) veya (.dll) dosyasının hedef yöntemi alınır. (Henüz otomatik değil)
- Yöntem içindeki tüm instruction'lar bir (.txt) dosyasına satır satır kaydedilir.
- DynamicMachine ise dinamik olarak yöntem oluşturur. İçine, önceden kaydedilen txt dosyasından okunan Instructions emitlenir. [Bunun için kesinlikle daha gelişmiş filter ve yöntemler gerekiyor.]
- Yöntem runtime oluşturulup çağrılır.

## Geliştiriliyor;

- Geliştirilmesi tamamlandığında mükemmel bir koruma sağlayacaktır.
- DynamicMachine ile korunan programlar dumplanamaz / cracklenmesi zordur.

## Hatırlatma

bin\debug içindeki dnlib kütüphanesini başvurulara eklemeyi unutmayın.


# güncellemeler

**V1.0**
- İlk kod yayınlandı.

**V1.1**
- En büyük geliştirmelerden biri, artık DynamicMachine bir class olarak hedef programın içine inject oluyor. (korunacak programın içine / ..ctor )
- Yeni OpCode destekleri.
