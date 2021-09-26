import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExeDetailComponent } from './exe-detail.component';

describe('ExeDetailComponent', () => {
  let component: ExeDetailComponent;
  let fixture: ComponentFixture<ExeDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
