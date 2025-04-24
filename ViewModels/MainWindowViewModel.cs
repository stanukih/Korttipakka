using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Korttipakka.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{

    public decimal res1 { get; set; } = 0;
    public decimal res2 { get; set; } = 0;
    public event PropertyChangedEventHandler? PropertyChanged;
        
    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public Kortti? activeKortti = null;
    public Pakka Korttit;
    
    public void initPakka()
    {
        Korttit = new Pakka();
        pakkaIvVisible = true;
        activeKorttiIvVisible = false;
        RaisePropertyChanged(nameof(pakkaIvVisible));
        otaKorttiButtonIsVisible = false;
        RaisePropertyChanged(nameof(otaKorttiButtonIsVisible));
        activeKorttiIvVisible = false;
        RaisePropertyChanged(nameof(activeKorttiIvVisible));
        RaisePropertyChanged(nameof(kortitListText));
        GC.Collect();
    }

    public void sekoita()
    {
        if (Korttit != null) Korttit.Sekoita();
        RaisePropertyChanged(nameof(kortitListText));
    }

    public void laitaKortti()
    {
        if (Korttit != null)
        {
            Console.WriteLine(1);
            if (Korttit.getLength() > 0)
            {
                Console.WriteLine(2);
                activeKortti = Korttit.getKortti();
                activeKorttiIvVisible = true;
                RaisePropertyChanged(nameof(activeKorttiIvVisible));
                RaisePropertyChanged(nameof(activeKorttiMaa));
                RaisePropertyChanged(nameof(activeKorttiNumeroarvo));
                RaisePropertyChanged(nameof(kortitListText));
                
                if (Korttit.getLength() == 0)
                {
                    Console.WriteLine(3);
                    pakkaIvVisible = false;
                    RaisePropertyChanged(nameof(pakkaIvVisible));
                    otaKorttiButtonIsVisible = true;
                    RaisePropertyChanged(nameof(otaKorttiButtonIsVisible));
                    
                    return;
                }
                
            }
        }
    }

    public bool otaKorttiButtonIsVisible { get; set; } = true;
    public bool pakkaIvVisible { get; set; } = false;
    public bool activeKorttiIvVisible { get; set; } = false;
    public string activeKorttiNumeroarvo
    {
        get
        {
            if (activeKortti != null)
            {
                return activeKortti.getNumeroarvoText();
            }
            return "";
        }
    }
    public char activeKorttiMaa
    {
        get
        {
            if (activeKortti != null)
            {
                return activeKortti.getMaaText();
            }
            return '\0';
        }
    }

    public string[] kortitListText
    {
        get
        {
            return Korttit.Tulosta_jarjestyksessaText().Split('\n');
        }
    }
}

public class Kortti
{
    public byte maa = 0;    
    /*
                            //0 = hertta,
                            //1 = ruutu,
                            //2 = risti
                            //3 = pata
                            */
    public byte numeroarvo = 0;  
    /*                            
                            //2 = 2
                            //3 = 3
                            //4 = 4
                            //5 = 5
                            //6 = 6
                            //7 = 7
                            //8 = 8
                            //9 = 9
                            //10 = 10
                            //11 = V
                            //12 = D
                            //13 = K
                            //14 = A
                            */
    public Kortti(byte maaI, byte numeroarvoI)
    {
        maa = maaI;
        numeroarvo = numeroarvoI;
    }

    public char getMaaText()
    {
        switch (maa)
        {
            case 0: return '\u2665';
            case 1: return '\u2666';
            case 2: return '\u2663';
            case 3: return '\u2660';
        }
        return '\0';
    }

    public string getNumeroarvoText()
    {
        if (numeroarvo < 11) return numeroarvo.ToString();
        switch (numeroarvo)
        {
            case 11: return "V";
            case 12: return "D";
            case 13: return "K";
            case 14: return "A";
        }

        return "";
    }
    
    public string getKorttiText()
    {
        return $"{getMaaText()} {getNumeroarvoText()}";
    }
}

public class Pakka
{
    Kortti[] korttit = new Kortti[52];

    public Pakka()
    {
        for (byte i = 0; i < 4; i++)
        {
            for (byte j = 0; j < 13; j++)
            {
                korttit[i * 13 + j] = new Kortti(i, (byte)(j+2));
            }
        }
    }

    public string Tulosta_jarjestyksessaText()
    {
        string res = "";
        for (int i = 0; i < korttit.Length; i++)
        {
            res += korttit[i].getKorttiText() + Environment.NewLine;
        }
        return res;
    }

    public void Sekoita(long kerta = 100000000)
    {
        if (korttit.Length<2) return;
        var rand = new Random();
        while (kerta > 0)
        {
            kerta--;
            int i = rand.Next(korttit.Length);
            int j = rand.Next(korttit.Length);
            (korttit[i], korttit[j]) = (korttit[j], korttit[i]);
        }
    }
    
    public void Reset()
    {
        for (byte i = 0; i < 4; i++)
        {
            for (byte j = 0; j < 13; j++)
            {
                korttit[i * 13 + j] = new Kortti(i, (byte)(j+2));
            }
        }
    }

    public Kortti getKortti()
    {
        if (korttit.Length > 0)
        {
            Kortti[] korttit2 = new Kortti[korttit.Length-1];
            for (byte i = 0; i < korttit2.Length; i++)
            {
                korttit2[i] = korttit[i+1];
            }

            Kortti temp = korttit[0];
            korttit = korttit2;
            return temp;            
        }
        throw new IndexOutOfRangeException();
    }

    public int getLength()
    {
        return korttit.Length;
    }
}