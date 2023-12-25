using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace MauiAppFirstProgram;

public partial class MainPage : ContentPage
{
    int count = 0;

    //private double currentVedimakX = 0;
    //private double currentVedimakY = 0;

    //private double currentSnitchX = 0;
    //private double currentSnitchY = 0;

    private bool isMovingSnitch = false;

    private bool isMovingDown = false;
    private bool isMovingUp = false;
    private double movementSpeed = 2;

    ObjectInPage<Image> VedimakInPage;
    ObjectInPage<Image> SnitchInPage;
    ObjectInPage<Image> WallInPage;

    private double imageY = 0;
    private double objectX = 0;

    //private Dictionary<Image, Tuple<double, double>> Images;

    public MainPage()
    {
        InitializeComponent();
        StartMovingImage();

        VedimakInPage = new ObjectInPage<Image>()
        {
            Object = Vedimak,
            CurrentX = Vedimak.TranslationX,
            CurrentY = Vedimak.TranslationY
        };
        SnitchInPage = new ObjectInPage<Image>()
        {
            Object = Snitch,
            CurrentX = Snitch.TranslationX,
            CurrentY = Snitch.TranslationY
        };
    }


    private async void MoveVedimakUp(object sender, EventArgs e)
    {
        Move(VedimakInPage, 0, -100);
        Move(SnitchInPage, 0, -100);
    }

    private async void MoveVedimakDown(object sender, EventArgs e)
    {
        //await MoveImage(Vedimak ,0, 100);
        Move(VedimakInPage, 0, 100);
        Move(SnitchInPage, 0, 100);
    }

    private async void ShootSnitch(object sender, EventArgs e)
    {
        await Move(SnitchInPage, 1200, 0);
        Snitch.TranslationX = 1200;


        if (CheckCollision())
        {
            Wall.ScaleTo(Wall.Scale - 0.1, 1000);
        }


        if (SnitchInPage.CurrentX > this.Width - 300)
        {
            Snitch.TranslateTo(0, VedimakInPage.CurrentY, 0);
            SnitchInPage.Restart(0, VedimakInPage.CurrentY);
            Snitch.TranslationX = 0;
            Snitch.TranslationY = VedimakInPage.CurrentY;
        }
    }

    private async Task Move(ObjectInPage<Image> image, double x, double y, uint animationDuration=500)
    {
        image.Move(x, y);
        await image.Object.TranslateTo(image.CurrentX, image.CurrentY, animationDuration);
        image.Object.TranslationX = image.CurrentX;
        image.Object.TranslationY = image.CurrentY;
    }

    private async Task StartMovingImage()
    {
        // Запуск бесконечной анимации
        Device.StartTimer(TimeSpan.FromMilliseconds(16), () =>
        {
            MoveImage();
            return true; // Возвращаем true для продолжения анимации
        });
    }

    private async Task MoveImage()
    {
        // Перемещение изображения вверх или вниз
        if (isMovingDown)
        {
            imageY += movementSpeed;
            if (imageY >= Height - Wall.HeightRequest -100)
            {
                isMovingDown = false;
            }
        }
        else
        {
            imageY -= movementSpeed;
            if (imageY <= -300)
            {
                isMovingDown = true;
            }
        }

        Wall.TranslationY = imageY;

        // Анимация перемещения изображения
        Wall.TranslateTo(Wall.TranslationX, imageY, 16); // 16 миллисекунд - пример частоты обновления
    }

    private bool CheckCollision()
    {
        if (this.WidthRequest - Snitch.TranslationX - Vedimak.WidthRequest < Wall.WidthRequest)
        {
            if (Wall.TranslationY + Wall.HeightRequest / 2 > Snitch.TranslationY &&
                Wall.TranslationY - Wall.HeightRequest / 2 < Snitch.TranslationY)
            {
                return true;
            }
            return false;
        }
        return false;
    }

}

internal class ObjectInPage<T> where T : Image
{
    public T Object { get; set; }
    public double CurrentX { get; set; }
    public double CurrentY { get; set; }

    public void Move(double x, double y)
    {
        CurrentX += x;
        CurrentY += y;
    }

    public void Restart(double x, double y)
    {
        CurrentX = x;
        CurrentY = y;
    }
}
