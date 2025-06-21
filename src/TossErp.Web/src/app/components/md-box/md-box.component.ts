import { Component, Input, ElementRef, Renderer2, OnInit } from '@angular/core';

@Component({
  selector: 'md-box',
  template: '<div [class]="boxClasses"><ng-content></ng-content></div>',
  standalone: true
})
export class MDBoxComponent implements OnInit {
  @Input() p: number = 0;
  @Input() px: number = 0;
  @Input() py: number = 0;
  @Input() pt: number = 0;
  @Input() pb: number = 0;
  @Input() pl: number = 0;
  @Input() pr: number = 0;
  @Input() m: number = 0;
  @Input() mx: number = 0;
  @Input() my: number = 0;
  @Input() mt: number = 0;
  @Input() mb: number = 0;
  @Input() ml: number = 0;
  @Input() mr: number = 0;
  @Input() display: string = '';
  @Input() position: string = '';
  @Input() top: string = '';
  @Input() left: string = '';
  @Input() right: string = '';
  @Input() bottom: string = '';
  @Input() width: string = '';
  @Input() height: string = '';
  @Input() minHeight: string = '';
  @Input() maxHeight: string = '';
  @Input() minWidth: string = '';
  @Input() maxWidth: string = '';
  @Input() borderRadius: string = '';
  @Input() bgColor: string = '';
  @Input() color: string = '';
  @Input() opacity: number = 1;
  @Input() shadow: string = '';
  @Input() customClass: string = '';

  boxClasses: string = '';

  constructor(private elementRef: ElementRef, private renderer: Renderer2) {}

  ngOnInit() {
    this.buildClasses();
    this.applyStyles();
  }

  private buildClasses() {
    const classes: string[] = ['md-box'];

    // Add spacing classes
    if (this.p > 0) classes.push(`p-${this.p}`);
    if (this.px > 0) classes.push(`px-${this.px}`);
    if (this.py > 0) classes.push(`py-${this.py}`);
    if (this.pt > 0) classes.push(`pt-${this.pt}`);
    if (this.pb > 0) classes.push(`pb-${this.pb}`);
    if (this.pl > 0) classes.push(`pl-${this.pl}`);
    if (this.pr > 0) classes.push(`pr-${this.pr}`);

    if (this.m > 0) classes.push(`m-${this.m}`);
    if (this.mx > 0) classes.push(`mx-${this.mx}`);
    if (this.my > 0) classes.push(`my-${this.my}`);
    if (this.mt > 0) classes.push(`mt-${this.mt}`);
    if (this.mb > 0) classes.push(`mb-${this.mb}`);
    if (this.ml > 0) classes.push(`ml-${this.ml}`);
    if (this.mr > 0) classes.push(`mr-${this.mr}`);

    // Add display class
    if (this.display) classes.push(`d-${this.display}`);

    // Add custom class
    if (this.customClass) classes.push(this.customClass);

    this.boxClasses = classes.join(' ');
  }

  private applyStyles() {
    const element = this.elementRef.nativeElement.querySelector('.md-box');

    if (this.position) this.renderer.setStyle(element, 'position', this.position);
    if (this.top) this.renderer.setStyle(element, 'top', this.top);
    if (this.left) this.renderer.setStyle(element, 'left', this.left);
    if (this.right) this.renderer.setStyle(element, 'right', this.right);
    if (this.bottom) this.renderer.setStyle(element, 'bottom', this.bottom);
    if (this.width) this.renderer.setStyle(element, 'width', this.width);
    if (this.height) this.renderer.setStyle(element, 'height', this.height);
    if (this.minHeight) this.renderer.setStyle(element, 'min-height', this.minHeight);
    if (this.maxHeight) this.renderer.setStyle(element, 'max-height', this.maxHeight);
    if (this.minWidth) this.renderer.setStyle(element, 'min-width', this.minWidth);
    if (this.maxWidth) this.renderer.setStyle(element, 'max-width', this.maxWidth);
    if (this.borderRadius) this.renderer.setStyle(element, 'border-radius', this.borderRadius);
    if (this.bgColor) this.renderer.setStyle(element, 'background-color', this.bgColor);
    if (this.color) this.renderer.setStyle(element, 'color', this.color);
    if (this.opacity !== 1) this.renderer.setStyle(element, 'opacity', this.opacity.toString());
    if (this.shadow) this.renderer.setStyle(element, 'box-shadow', this.shadow);
  }
}
