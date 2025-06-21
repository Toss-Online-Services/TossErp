import { Component, Input } from '@angular/core';

@Component({
  selector: 'md-typography',
  template: `
    <h1 *ngIf="variant === 'h1'" [class]="typographyClasses"><ng-content></ng-content></h1>
    <h2 *ngIf="variant === 'h2'" [class]="typographyClasses"><ng-content></ng-content></h2>
    <h3 *ngIf="variant === 'h3'" [class]="typographyClasses"><ng-content></ng-content></h3>
    <h4 *ngIf="variant === 'h4'" [class]="typographyClasses"><ng-content></ng-content></h4>
    <h5 *ngIf="variant === 'h5'" [class]="typographyClasses"><ng-content></ng-content></h5>
    <h6 *ngIf="variant === 'h6'" [class]="typographyClasses"><ng-content></ng-content></h6>
    <p *ngIf="variant === 'body1'" [class]="typographyClasses"><ng-content></ng-content></p>
    <p *ngIf="variant === 'body2'" [class]="typographyClasses"><ng-content></ng-content></p>
    <span *ngIf="variant === 'button'" [class]="typographyClasses"><ng-content></ng-content></span>
    <span *ngIf="variant === 'caption'" [class]="typographyClasses"><ng-content></ng-content></span>
    <span *ngIf="variant === 'overline'" [class]="typographyClasses"><ng-content></ng-content></span>
  `,
  standalone: true
})
export class MDTypographyComponent {
  @Input() variant: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'body1' | 'body2' | 'button' | 'caption' | 'overline' = 'body1';
  @Input() color: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark' | 'text' | 'white' = 'text';
  @Input() fontWeight: 'light' | 'regular' | 'medium' | 'bold' = 'regular';
  @Input() textTransform: 'none' | 'capitalize' | 'uppercase' | 'lowercase' = 'none';
  @Input() verticalAlign: 'unset' | 'baseline' | 'sub' | 'super' | 'text-top' | 'text-bottom' | 'middle' | 'top' | 'bottom' = 'unset';
  @Input() opacity: number = 1;
  @Input() textGradient: boolean = false;
  @Input() customClass: string = '';

  get typographyClasses(): string {
    const classes: string[] = ['md-typography'];

    classes.push(`md-typography--${this.variant}`);
    classes.push(`md-typography--${this.color}`);
    classes.push(`md-typography--${this.fontWeight}`);
    classes.push(`md-typography--${this.textTransform}`);
    classes.push(`md-typography--${this.verticalAlign}`);

    if (this.textGradient) classes.push('md-typography--gradient');
    if (this.customClass) classes.push(this.customClass);

    return classes.join(' ');
  }
}
