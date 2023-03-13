import {Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../../environments/environment";
import {AdminService} from "../../_services/admin.service";
import {Photo} from "../../_models/photo";

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  baseUrl = environment.apiUrl;
  unapprovedPhotos: Photo[] = [];

  constructor(private http: HttpClient, private adminService: AdminService) {
  }

  ngOnInit(): void {
    this.getUnapprovedPhotos().subscribe(photos => {
      this.unapprovedPhotos = photos;
    })
  }

  getUnapprovedPhotos() {
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate');
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
        next: () => {
          this.unapprovedPhotos = this.unapprovedPhotos.filter(x => x.id != photoId);
        }
      }
    )
  }

  deletePhoto(photoId: number) {
    this.adminService.deletePhoto(photoId).subscribe({
      next: () => {
        this.unapprovedPhotos = this.unapprovedPhotos.filter(x => x.id != photoId);
      }
    })
  }
}
