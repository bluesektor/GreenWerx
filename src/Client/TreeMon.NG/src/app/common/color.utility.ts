
export class Color {

    // To help create a heatmap of colors.
    //
   public static GetRgb( low: number,  high: number,  value: number): string {

        let r, g, b = 0;

        let range = high - low;
        if (range <= 0) {
            return '#' + Color.hex(r) + Color.hex(g) + Color.hex(b);
        }

        let pct = (( value - low) * 100 ) / (high - low);
        console.log('low:', low, 'high:', high, 'value:', value);
        console.log('pct:', pct);

        if ( pct > 80 ) { // red
            r = 255;
            g = (100 - pct);
            b = 0;
        } else if (pct <= 80 && pct > 60) { // yellow
            r = 240;
            g = 255 - (80 - pct); // decrease the brightness the lower the pct in this range.
            b = 0;
        }  else if (pct <= 60 && pct > 40) {  // green
            r = 0;
            g = 255;
            b = 60 - pct;
        } else if (pct <= 40 && pct > 20) { // blue
            r = 40 - pct;
            g = 0;
            b = 255;
        } else {
            r = 106 + (20 - pct);
            g = 25;
            b = 255;
        }
        console.log('rgb:', r, g, b );
        return '#' + Color.hex(r) + Color.hex(g) + Color.hex(b);
    }

    public static hex(x): any {
     return ('0' + parseInt(x, 10).toString(16)).slice(-2);
    }
}
