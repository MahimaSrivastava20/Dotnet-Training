import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-welcome',
  imports: [RouterModule, CommonModule],
  templateUrl: './welcome.html',
  styleUrl: './welcome.css',
})
export class Welcome {
  activeFeature = signal<string | null>(null);
  selectedInsurance = signal<any>(null);

  insuranceTypes = [
    { id: 'term-life', name: 'Term Life', icon: 'https://img.icons8.com/color/96/shield.png', badge: 'Save 15%', badgeColor: 'green', desc: 'Secure your family\'s financial future with high coverage at affordable premiums. Ideal for primary breadwinners.' },
    { id: 'health', name: 'Health', icon: 'https://img.icons8.com/color/96/heart-health.png', badge: 'Best Value', badgeColor: 'blue', desc: 'Comprehensive medical coverage covering hospitalization, day-care procedures, and pre/post medical expenses.' },
    { id: 'investment', name: 'Investment', icon: 'https://img.icons8.com/color/96/money-bag.png', desc: 'Grow your wealth steadily over time with market-linked or guaranteed return plans tailored for your goals.' },
    { id: 'car', name: 'Car', icon: 'https://img.icons8.com/color/96/car.png', badge: 'Price Match', badgeColor: 'amber', desc: 'Protect your vehicle against accidents, theft, and third-party liabilities with quick cashless claim settlements.' },
    { id: '2-wheeler', name: '2 Wheeler', icon: 'https://img.icons8.com/color/96/motorcycle.png', badge: 'Up to 85% Off', badgeColor: 'purple', desc: 'Mandatory third-party and comprehensive covers for your bike or scooter to keep you legally compliant and safe.' },
    { id: 'family-health', name: 'Family Health', icon: 'https://img.icons8.com/color/96/family.png', desc: 'A single umbrella health cover for your entire family, providing peace of mind against medical emergencies.' },
    { id: 'travel', name: 'Travel', icon: 'https://img.icons8.com/color/96/airplane-take-off.png', desc: 'Travel the world worry-free. Covers flight delays, lost baggage, and medical emergencies abroad.' },
    { id: 'term-women', name: 'Term (Women)', icon: 'https://img.icons8.com/color/96/businesswoman.png', desc: 'Specialized term life insurance with discounted premium rates designed specifically for women.' },
    { id: 'return-premium', name: 'Return of Premium', icon: 'https://img.icons8.com/color/96/cash-in-hand.png', desc: 'Get 100% of your premiums back if you outlive the policy term. A great combination of protection and savings.' },
    { id: 'guaranteed-return', name: 'Guaranteed Return', icon: 'https://img.icons8.com/color/96/safe.png', badge: '7.3% Returns', badgeColor: 'teal', desc: 'Zero risk, fixed income plans. Lock in your money at high interest rates for predictable growth.' },
    { id: 'child-savings', name: 'Child Savings', icon: 'https://img.icons8.com/color/96/teddy-bear.png', badge: 'Premium Waiver', badgeColor: 'rose', desc: 'Fund your child\'s higher education and marriage. Premiums are waived off in case of parent\'s unfortunate demise.' },
    { id: 'retirement', name: 'Retirement', icon: 'https://img.icons8.com/color/96/elderly-person.png', desc: 'Build a retirement corpus to ensure a steady stream of income and financial independence in your golden years.' },
    { id: 'employee-group', name: 'Employee Group', icon: 'https://img.icons8.com/color/96/conference-call.png', desc: 'Group health and life insurance solutions for corporate teams to attract and retain top talent.' },
    { id: 'home', name: 'Home Insurance', icon: 'https://img.icons8.com/color/96/home.png', desc: 'Protect your house and its contents against fire, natural disasters, burglary, and unforeseen damages.' }
  ];

  toggleFeature(feature: string) {
    this.activeFeature.update(f => f === feature ? null : feature);
  }

  showInsuranceDetails(insurance: any, event: Event) {
    event.preventDefault(); // Prevent navigation
    this.selectedInsurance.set(insurance);
  }

  closeModal() {
    this.selectedInsurance.set(null);
  }
}

